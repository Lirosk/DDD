from PIL import Image
from pytesseract import pytesseract

def recognize_text(image_path):
    # Define path to tesseract.exe
    path_to_tesseract = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

    # Point pytesseract to tesseract.exe
    pytesseract.tesseract_cmd = path_to_tesseract

    # Open image with PIL
    img = Image.open(image_path)

    # Extract text from image
    text = pytesseract.image_to_string(img)

    return text
