from PIL import Image
from pytesseract import pytesseract

from PIL import Image, ImageDraw, ImageFont
import cv2
import pytesseract
import numpy as np

def recognize_text(image_path):
    path_to_tesseract = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
    pytesseract.pytesseract.tesseract_cmd = path_to_tesseract

    img = Image.open(image_path)

    text = pytesseract.image_to_string(img)

    return text

def extract_text_color(image):
    grayscale_image = image.convert("L")

    threshold = 128
    binary_image = grayscale_image.point(lambda p: p > threshold and 255)

    opencv_image = cv2.cvtColor(np.array(binary_image), cv2.COLOR_RGB2BGR)

    edges = cv2.Canny(opencv_image, 50, 150)
    contours, _ = cv2.findContours(edges, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    text_color = None
    for contour in contours:
        x, y, w, h = cv2.boundingRect(contour)
        text_region = image.crop((x, y, x + w, y + h))

        text_region_color = text_region.getpixel((0, 0))
        if text_color is None:
            text_color = text_region_color
        elif text_region_color != text_color:
            text_color = None
            break

    return text_color

def insert_translated_text(image_path, translated_text):
    image = Image.open(image_path)

    width, height = image.size

    font_size = int(min(width, height) * 0.1)
    text_color = extract_text_color(image)
    font = ImageFont.truetype("path_to_font.ttf", font_size)
    draw = ImageDraw.Draw(image)

    text_width, text_height = draw.textsize(translated_text, font=font)
    text_x = (width - text_width) // 2
    text_y = (height - text_height) // 2

    draw.text((text_x, text_y), translated_text, fill=text_color, font=font)

    image.save("path_to_final_image.png")

original_text = recognize_text("samples/test1.png")
translated_text = original_text # заменить
insert_translated_text("samples/no_text_test1.png", translated_text)
