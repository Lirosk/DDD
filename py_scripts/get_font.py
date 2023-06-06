import requests
import pytesseract
from PIL import Image


# Определите путь к изображению с текстом
image_path = "samples/rectangle_2.png"

# Распознайте текст на изображении с помощью pytesseract
path_to_tesseract = r"C:\Program Files\Tesseract-OCR\tesseract.exe"
pytesseract.pytesseract.tesseract_cmd = path_to_tesseract

# Open image with PIL
img = Image.open(image_path)

# Extract text from image
text = pytesseract.image_to_string(img)

# Определите API-ключ WhatFontIs
api_key = "afd6dd11f8424a7f843ed1f5e64b77a2f850ae508a87ad89eb6c46f55ef4d70a"

# Определите URL для отправки запроса к WhatFontIs API
url = "https://www.whatfontis.com/api/"

# Определите параметры запроса
params = {
    "file": {
        "FONT": {
            "API_KEY": api_key,
            "INFO": {
                "TITLE": text
            }
        }
    },
    "limit": 2
}

# Отправьте запрос к WhatFontIs API
response = requests.get(url, params=params)

# Обработайте ответ API и извлеките информацию о шрифтах
if response.status_code == 200:
    font_info = response.json()
    for font in font_info:
        title = font["title"]
        font_url = font["url"]
        font_image = font["image"]
        # Обработайте информацию о шрифте
        print(f"Найден шрифт: {title}")
        print(f"URL шрифта: {font_url}")
        print(f"Пример изображения шрифта: {font_image}")
else:
    error_code = response.status_code
    print(f"Ошибка при обращении к WhatFontIs API. Код ошибки: {error_code}")
