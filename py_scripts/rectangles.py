import cv2
import numpy as np
import os
from PIL import Image
from pytesseract import pytesseract
from sklearn.cluster import KMeans
import math

import keras_ocr

from googletrans import Translator



# Create a keras-ocr pipeline for text recognition
pipeline = keras_ocr.pipeline.Pipeline()

path_to_tesseract = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
pytesseract.tesseract_cmd = path_to_tesseract

def recognize_text(image_path):
    # Open image with PIL
    img = Image.open(image_path)

    # Extract text from image
    text = pytesseract.image_to_string(img)

    return text

def midpoint(x1, y1, x2, y2):
    x_mid = int((x1 + x2) / 2)
    y_mid = int((y1 + y2) / 2)
    return (x_mid, y_mid)

def detect_text_color(image, x, y, w, h):
    roi = image[y:y+h, x:x+w]  # Extract the region of interest (rectangle)
    hsv_roi = cv2.cvtColor(roi, cv2.COLOR_BGR2HSV)  # Convert the region of interest to HSV color space

    # Reshape the HSV image to a 2D array of pixels
    pixels = hsv_roi.reshape(-1, 3)

    # Apply KMeans clustering with k=2 to separate the foreground and background colors
    kmeans = KMeans(n_clusters=2)
    kmeans.fit(pixels)

    # Determine the dominant cluster (foreground or background) based on the cluster centers' values
    cluster_centers = kmeans.cluster_centers_
    foreground_cluster = np.argmax(cluster_centers.sum(axis=1))  # Cluster with higher sum of values is the foreground

    # Set the text color based on the dominant cluster
    if foreground_cluster == 0:
        text_color = (255, 255, 255)  # White color for dark background
    else:
        text_color = (0, 0, 0)  # Black color for light background

    return text_color

def add_translated_text(image, text, x, y, w, h, text_color):
    max_font_size = int(h * 0.8)  # Maximum font size based on rectangle height

    font_size = max_font_size
    font = cv2.FONT_HERSHEY_SIMPLEX
    text_width = cv2.getTextSize(text, font, font_size, 1)[0][0]

    while text_width > w and font_size > 1:
        font_size -= 1
        text_width = cv2.getTextSize(text, font, font_size, 1)[0][0]

    text_x = x + int((w - text_width) / 2)
    text_y = y + h - int((h - font_size) / 2)

    cv2.putText(image, text, (text_x, text_y), font, font_size, text_color, 2)

def inpaint_text(img_path, pipeline):
    img = keras_ocr.tools.read(img_path)
    prediction_groups = pipeline.recognize([img])
    mask = np.zeros(img.shape[:2], dtype="uint8")
    for box in prediction_groups[0]:
        x0, y0 = box[1][0]
        x1, y1 = box[1][1]
        x2, y2 = box[1][2]
        x3, y3 = box[1][3]
        x_mid0, y_mid0 = midpoint(x1, y1, x2, y2)
        x_mid1, y_mid1 = midpoint(x0, y0, x3, y3)
        thickness = int(math.sqrt((x2 - x1) ** 2 + (y2 - y1) ** 2))
        cv2.line(mask, (x_mid0, y_mid0), (x_mid1, y_mid1), 255, thickness)
    inpainted_img = cv2.inpaint(img, mask, 7, cv2.INPAINT_NS)
    return inpainted_img

def save_rectangles(image_path, contours):
    folder_name = 'rectangles'
    filename = os.path.basename(image_path)
    filename_without_extension = os.path.splitext(filename)[0]
    save_folder = os.path.join(folder_name, filename_without_extension + '_rectangles')

    if not os.path.exists(folder_name):
        os.makedirs(folder_name)

    if not os.path.exists(save_folder):
        os.makedirs(save_folder)

    original_image = cv2.imread(image_path)
    image_with_rect = original_image.copy()

    for idx, contour in enumerate(contours):
        x, y, w, h = cv2.boundingRect(contour)
        x = 2 * x
        y = 2 * y
        w = 2 * w
        h = 2 * h
        cv2.rectangle(image_with_rect, (x, y), (x + w - 1, y + h - 1), (0, 255, 0), 2)
        rectangle_img = original_image[y:y + h, x:x + w]
        save_path = os.path.join(save_folder, f'rectangle_{idx + 1}.png')
        cv2.imwrite(save_path, rectangle_img)

        # Perform OCR on the rectangle image
        text = recognize_text(save_path)

        if not text:
            os.remove(save_path)
        else:
            # Remove text from the saved rectangle image
            text_removed_img = inpaint_text(save_path, pipeline)
            text_removed_save_path = os.path.join(save_folder, f'no_text_rect_{idx + 1}.png')
            cv2.imwrite(text_removed_save_path, text_removed_img)

            # Translate the text
            # translator = Translator()
            # translation = translator.translate(text, dest="ru")
            # translated_text = translation.text

            # Add translated text with automatic font size and font
            # text_color = detect_text_color(image_with_rect, x, y, w, h)
            # add_translated_text(image_with_rect, translated_text, x, y, w, h, text_color)

            # Save the translated text rectangle
            # translated_rect_path = os.path.join(save_folder, f'translated_rect_{idx + 1}.png')
            # cv2.imwrite(translated_rect_path, image_with_rect)

    image_with_rect_path = os.path.join(folder_name, filename_without_extension + '_with_rect.png')
    cv2.imwrite(image_with_rect_path, image_with_rect)


folder_path = 'samples'

for filename in os.listdir(folder_path):
    if filename.endswith('.png') or filename.endswith('.jpg') or filename.endswith('.jpeg'):
        image_path = os.path.join(folder_path, filename)
        large = cv2.imread(image_path)
        small = cv2.pyrDown(large)
        gray = cv2.cvtColor(small, cv2.COLOR_BGR2GRAY)
        kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3))
        grad = cv2.morphologyEx(gray, cv2.MORPH_GRADIENT, kernel)
        _, bw = cv2.threshold(grad, 0.0, 255.0, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

        kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (7, 4))

        connected = cv2.morphologyEx(bw, cv2.MORPH_CLOSE, kernel)

        contours, hierarchy = cv2.findContours(connected.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
        mask = np.zeros(bw.shape, dtype=np.uint8)

        valid_contours = []

        for idx in range(len(contours)):
            x, y, w, h = cv2.boundingRect(contours[idx])
            mask[y:y+h, x:x+w] = 0
            cv2.drawContours(mask, contours, idx, (255, 255, 255), -1)
            r = float(cv2.countNonZero(mask[y:y+h, x:x+w])) / (w * h)
            if r > 0.1 and w > 1 and h > 1:
                x = 2 * x
                y = 2 * y
                w = 2 * w
                h = 2 * h
                cv2.rectangle(large, (x, y), (x+w-1, y+h-1), (0, 255, 0), 2)
                valid_contours.append(contours[idx])

        save_rectangles(image_path, valid_contours)

        cv2.namedWindow('rects', cv2.WINDOW_NORMAL)
        cv2.imshow('rects', large)
        cv2.waitKey()

cv2.destroyAllWindows()
