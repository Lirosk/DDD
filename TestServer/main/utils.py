import io
import os
from PIL import Image, ImageDraw, ImageFont
import cv2
import numpy as np
import typing
from googletrans import Translator 
from google.cloud import vision_v1 as vision
from django.conf import settings
import requests


translator = Translator()
google_vision_client = vision.ImageAnnotatorClient()


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
        image_bytes = remove_text(image_bytes, translated_text_with_coords[1][1:])
    
    return image_bytes


def remove_text(image_bytes: bytes, coords: typing.Tuple[typing.Tuple[int, int], typing.Tuple[int, int]]) -> bytes:
    nparr = np.frombuffer(image_bytes, np.uint8)

    image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    top_left, bottom_right = coords

    mask = np.zeros(image.shape[:2], dtype=np.uint8)
    cv2.rectangle(mask, top_left, bottom_right, (255), cv2.FILLED)

    p1, p2 = coords

    result = cv2.inpaint(image, mask, abs(p1[1] - p2[1]), cv2.INPAINT_TELEA)

    retval, buffer = cv2.imencode('.png', result)
    result_bytes = buffer.tobytes()

    return result_bytes

def recognize_font_style(image_bytes: bytes, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]) -> str:
    api_key = settings.WHATFONTIS_API_KEY
    url = "https://www.whatfontis.com/api/"

    font_styles = []
    for text_with_coords in texts_with_coords:
        title = text_with_coords[0]

        params = {
            "file": {
                "FONT": {
                    "API_KEY": api_key,
                    "INFO": {
                        "TITLE": title
                    }
                }
            },
            "limit": 1
        }

        response = requests.get(url, params=params)

        if response.status_code == 200:
            font_info = response.json()
            if font_info:
                font_style = font_info[0].get("title")
                font_styles.append(font_style)
        else:
            error_code = response.status_code
            print(f"Error accessing WhatFontIs API. Error code: {error_code}")
            return []

    return font_styles


def recognize_text_colors(image_bytes: bytes, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]]):
    colors = []

    for text_with_coords in texts_with_coords:
        colors.append(recognize_text_color(image_bytes, text_with_coords[1:]))

    return colors


def recognize_text_color(image_bytes: bytes, coords: typing.Tuple[typing.Tuple[int, int], typing.Tuple[int, int]]):
    image = Image.open(io.BytesIO(image_bytes))

    p1, p2 = coords

    text_color = None
    text_region = image.crop((p1[0], p1[1], p2[0], p2[1]))

    text_region_color = text_region.getpixel((0, 0))
    if text_color is None:
        text_color = text_region_color
    elif text_region_color != text_color:
        text_color = (255,) * 3

    return text_color


def place_texts_into_image(image_bytes: bytes, texts_with_coords: typing.List[typing.Tuple[str, typing.Tuple[int, int], typing.Tuple[int, int]]], translated_texts: typing.List[str]) -> bytes:
    for translated_text_with_coords in zip(translated_texts, texts_with_coords):
        if translated_text_with_coords[0] == translated_text_with_coords[1][0]:
            continue
        image_bytes = place_text_into_image(image_bytes, translated_text_with_coords[0], translated_text_with_coords[1][1:])
    
    return image_bytes


def place_text_into_image(image_bytes: bytes, translated_text: str, coords: typing.Tuple[typing.Tuple[int, int], typing.Tuple[int, int]]): 
    image = Image.open(io.BytesIO(image_bytes))

    x1, y1 = coords[0]
    x2, y2 = coords[1]

    font_size = abs(y2 - y1)
    font_path = os.path.join(settings.STATIC_ROOT, "calibri-regular.ttf")

    text_color = recognize_text_color(image_bytes, coords)
    image_with_text = image.copy()
    draw = ImageDraw.Draw(image_with_text)
    font = ImageFont.truetype(font_path, font_size)

    text_width, text_height = draw.textsize(translated_text, font=font)
    
    text_x = x1 + (x2 - x1) // 2 - text_width // 2
    text_y = y1 + (y2 - y1) // 2 - text_height // 2

    draw.text((text_x, text_y), translated_text, fill=text_color, font=font)

    with io.BytesIO() as ms:
        image_with_text.save(ms, format="PNG")
        image_bytes = ms.getvalue()

        return image_bytes