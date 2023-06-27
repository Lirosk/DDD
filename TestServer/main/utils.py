import io
import os
from PIL import Image, ImageDraw, ImageFont, ImageOps
import cv2
import numpy as np
import typing
from googletrans import Translator
from google.cloud import vision_v1 as vision
from django.conf import settings
import requests
import string
import math
import numpy as np
from PIL import ImageFilter
from tensorflow.keras.utils import img_to_array
from keras.models import load_model
from django.conf import settings
import os
import colorgram


model = load_model(os.path.join(settings.STATIC_ROOT, 'top_model.h5'))
translator = Translator()
google_vision_client = vision.ImageAnnotatorClient()


def blur_image(pil_im):
    blur_img = pil_im.filter(ImageFilter.GaussianBlur(radius=1))
    return blur_img


def rev_conv_label(label):
    if label == 0 :
        return 'Lato'
    elif label == 1:
        return 'Raleway'
    elif label == 2 :
        return 'Roboto'
    elif label == 3 :
        return 'Sansation'
    elif label == 4:
        return 'Roboto'
        # return 'Walkway'
    elif label == 5:
        return 'Roboto'
        # return 'Cascadia Code'
    elif label == 6:
        return 'Open Sans'
    elif label == 7:
        return 'Times New Roman'
    elif label == 8:
        return 'Calibri'


def recognize_font_style(image: Image):
    image = image.convert("RGB")

    if is_image_dark(image):
        image = ImageOps.invert(image)

    grey_image = image.convert('L')

    blurred_image=blur_image(grey_image)

    resized_image = blurred_image.resize((105, 105))
    
    org_img = img_to_array(resized_image)

    data=[]
    data.append(org_img)
    data = np.asarray(data, dtype="float") / 255.0

    y = model.predict(data)

    y_0 = y[0]
    label = rev_conv_label(np.argmax(y_0))

    return label


def recognize_text_from_image(image_bytes: bytes) -> typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]:
    response = google_vision_client.annotate_image({
        'image': {'content': image_bytes},
        'features': [
            {
                'type_': vision.Feature.Type.TEXT_DETECTION,
            },
        ]
    })

    text_annotations = response.text_annotations
    texts_with_coords = []

    if text_annotations:
        for annotation in text_annotations[1:]:
            description = annotation.description
            vertices = annotation.bounding_poly.vertices
            top_left = (vertices[0].x, vertices[0].y)
            bottom_right = (vertices[2].x, vertices[2].y)
            texts_with_coords.append((description, top_left, bottom_right))

    return texts_with_coords


def translate_texts(source_texts: typing.List[str], target_language: str) -> typing.List[str]:
    res = []
    for source_text in source_texts:
        res.append(translate_text(source_text, target_language))

    return res


def translate_text(source_text: str, target_language: str) -> str:
    translation = translator.translate(source_text, dest=target_language)
    translated_text = translation.text
    return translated_text


def remove_texts(image_bytes: bytes, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]], translated_texts: typing.List[str]) -> bytes:
    for translated_text_with_coords in zip(translated_texts, texts_with_coords):
        if translated_text_with_coords[0] == translated_text_with_coords[1][0]:
            continue
        image_bytes = remove_text(
            image_bytes, translated_text_with_coords[1][1:])

    return image_bytes


def remove_text(image_bytes: bytes, coords: typing.Tuple[typing.Tuple[int, int], typing.Tuple[int, int]]) -> bytes:
    nparr = np.frombuffer(image_bytes, np.uint8)

    image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    p1, p2 = coords

    coeff = 1.2

    p1 = (p1[0], int(p1[1] * (2 - coeff)))
    p2 = (p2[0], int(p2[1] * coeff))

    mask = np.zeros(image.shape[:2], dtype=np.uint8)
    cv2.rectangle(mask, p1, p2, 255, cv2.FILLED)

    result = cv2.inpaint(image, mask, abs(p1[1] - p2[1]), cv2.INPAINT_TELEA)

    retval, buffer = cv2.imencode('.png', result)
    result_bytes = buffer.tobytes()

    return result_bytes


# def recognize_font_style(image_bytes: bytes, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]) -> str:
#     api_key = settings.WHATFONTIS_API_KEY
#     url = "https://www.whatfontis.com/api/"

#     font_styles = []
#     for text_with_coords in texts_with_coords:
#         title = text_with_coords[0]

#         params = {
#             "file": {
#                 "FONT": {
#                     "API_KEY": api_key,
#                     "INFO": {
#                         "TITLE": title
#                     }
#                 }
#             },
#             "limit": 1
#         }

#         response = requests.get(url, params=params)

#         if response.status_code == 200:
#             font_info = response.json()
#             if font_info:
#                 font_style = font_info[0].get("title")
#                 font_styles.append(font_style)
#         else:
#             error_code = response.status_code
#             print(f"Error accessing WhatFontIs API. Error code: {error_code}")
#             return []

#     return font_styles


def recognize_text_colors(image: Image, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]):
    colors = []

    for text_with_coords in texts_with_coords:
        colors.append(recognize_text_color(image.crop(texts_with_coords[1:])))

    return colors


def recognize_text_color(image: Image):
    colors = colorgram.extract(image, 3)

    if is_image_dark(image):
        return colors[1 if np.mean(colors[1].rgb) > np.mean(colors[2].rgb) else 2].rgb

    return colors[1 if np.mean(colors[1].rgb) < np.mean(colors[2].rgb) else 2].rgb


def place_texts_into_image(source_image_bytes: bytes, image_with_removed_text_bytes: bytes, dpiY: float, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]], translated_texts: typing.List[str]) -> bytes:
    for translated_text_with_coords in zip(translated_texts, texts_with_coords):
        if translated_text_with_coords[0] == translated_text_with_coords[1][0]:
            continue
        image_with_removed_text_bytes = place_text_into_image(source_image_bytes,
            image_with_removed_text_bytes, dpiY, translated_text_with_coords[0], translated_text_with_coords[1][1:])

    return image_with_removed_text_bytes


def place_text_into_image(source_image_bytes: bytes, image_bytes: bytes, dpiY: float, translated_text: str, coords: typing.Tuple[typing.Tuple[int, int], typing.Tuple[int, int]]):
    source_image = Image.open(io.BytesIO(source_image_bytes))
    image = Image.open(io.BytesIO(image_bytes)).convert("RGB")

    x1, y1 = coords[0]
    x2, y2 = coords[1]

    cropped_source_image = source_image.crop((x1, y1, x2, y2))

    font_style = recognize_font_style(cropped_source_image)

    font_size = math.ceil(abs(y2 - y1))
    font_path = os.path.join(settings.STATIC_ROOT, 'fonts', font_style + ".ttf")

    text_color = recognize_text_color(cropped_source_image)
    # text_color = (0, 0, 0)
    image_with_text = image.copy()
    draw = ImageDraw.Draw(image_with_text)
    font = ImageFont.truetype(font_path, font_size)

    text_sizes = draw.textsize(translated_text, font)
    available_bound = abs(x2 - x1)
    while text_sizes[0] < available_bound * 0.9:
        font_size += 1
        font = ImageFont.truetype(font_path, font_size)
        text_sizes = draw.textsize(translated_text, font)

    while text_sizes[0] > available_bound:
        font_size -= 1
        font = ImageFont.truetype(font_path, font_size)
        text_sizes = draw.textsize(translated_text, font)
    
    # text_width, text_height = draw.textsize(translated_text, font=font)

    # text_x = x1 + (x2 - x1) // 2 - text_width // 2
    # text_y = y1 + (y2 - y1) // 2 - text_height // 2

    result_x1, result_y1 = x1, y1

    bound_height = abs(y2 - y1)
    result_y1 += (bound_height - text_sizes[1]) / 2

    draw.text((result_x1, result_y1), translated_text, fill=text_color, font=font)

    print('Translated text:', translated_text)
    print('Font style:', font_style)
    print('Font size:', font_size)
    print('Text color:', text_color)
    print()

    with io.BytesIO() as ms:
        image_with_text.save(ms, format="PNG")
        image_bytes = ms.getvalue()

        return image_bytes


def separate_texts_into_strings(texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]) -> typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]:
    strings = []
    
    l = len(texts_with_coords)
    current = texts_with_coords[0]
    i = 1
    while i < l:
        checked = texts_with_coords[i]
        res, new_text_with_coords = are_on_same_line(current, checked)
        if res:
            current = new_text_with_coords
        else:
            strings.append(current)
            current = texts_with_coords[i]

        i += 1
    else:
        strings.append(current)
    
    return strings


def are_on_same_line(
        text_with_coords_1: typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]],
        text_with_coords_2: typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]
) -> typing.Tuple[bool, typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]:
    text1, *coords1 = text_with_coords_1
    text2, *coords2 = text_with_coords_2

    # x1, y1 = coords1[0]
    # w1 = coords1[1][0] - x1
    # h1 = coords1[1][1] - y1

    # x2, y2 = coords2[0]
    # w2 = coords2[1][0] - x2
    # h2 = coords2[1][1] - y2

    special_symbols = "{}[]()#№;:\'\"<>/+=-*&^%$@1234567890"

    if text1 in special_symbols or text2 in special_symbols:
        return False, ['', (-1, -1), (-1, -1)]

    distance_between = abs(coords1[1][0] - coords2[0][0])

    height1 = abs(coords1[0][1] - coords1[1][1])
    height2 = abs(coords2[0][1] - coords2[1][1])

    height = min(height1, height2)

    new_x1 = coords1[0][0]
    new_x2 = coords2[1][0]

    new_y1 = (coords1[0][1] + coords2[0][1]) // 2
    new_y2 = (coords1[1][1] + coords2[1][1]) // 2

    new_text = ""
    letters_for_union = string.ascii_letters + '_'
    if text1[-1] in letters_for_union and text2[0] in letters_for_union:
        new_text = text1 + ' ' + text2
    else:
        new_text = text1 + text2

    return distance_between < height, (new_text, (new_x1, new_y1), (new_x2, new_y2))

def is_image_dark(image: Image):
    colors = colorgram.extract(image, 2)

    mean_brightness = np.mean(np.array(image))

    main_color = colors[0].rgb
    main_color_brightness = np.mean(main_color)

    second_color_brightness = np.mean(colors[1].rgb)

    return mean_brightness > main_color_brightness and second_color_brightness > main_color_brightness