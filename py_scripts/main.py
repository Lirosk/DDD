import cv2
import numpy as np
import os
import pytesseract
import matplotlib.pyplot as plt
import keras_ocr
import math


def save_rectangles(image_path, contours):
    folder_name = 'py_scripts/rectangles'
    filename = os.path.basename(image_path)
    filename_without_extension = os.path.splitext(filename)[0]
    save_folder = os.path.join(folder_name, filename_without_extension + '_rectangles')

    if not os.path.exists(folder_name):
        os.makedirs(folder_name)

    if not os.path.exists(save_folder):
        os.makedirs(save_folder)

    original_image = cv2.imread(image_path)
    image_with_rect = original_image.copy()

    pipeline = keras_ocr.pipeline.Pipeline()

    def midpoint(x1, y1, x2, y2):
        x_mid = int((x1 + x2) / 2)
        y_mid = int((y1 + y2) / 2)
        return (x_mid, y_mid)

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

    for idx, contour in enumerate(contours):
        x, y, w, h = cv2.boundingRect(contour)
        x = 2 * x
        y = 2 * y
        w = 2 * w
        h = 2 * h
        cv2.rectangle(image_with_rect, (x, y), (x + w - 1, y + h - 1), (0, 255, 0), 2)
        rectangle_img = original_image[y:y + h, x:x + w]

        pytesseract.pytesseract.tesseract_cmd = r"C:\Program Files\Tesseract-OCR\tesseract.exe"
        text = pytesseract.image_to_string(rectangle_img)

        if not text:
            continue

        save_path = os.path.join(save_folder, f'rectangle_{idx + 1}.png')
        cv2.imwrite(save_path, rectangle_img)

        text_removed_img = inpaint_text(save_path, pipeline)
        text_removed_save_path = os.path.join(save_folder, f'rectangle_{idx + 1}_no_text.png')
        cv2.imwrite(text_removed_save_path, text_removed_img)

    image_with_rect_path = os.path.join(folder_name, filename_without_extension + '_with_rect.png')
    cv2.imwrite(image_with_rect_path, image_with_rect)


folder_path = 'py_scripts/samples'

for filename in os.listdir(folder_path):
    if filename.endswith('.png') or filename.endswith('.jpg') or filename.endswith('.jpeg'):
        image_path = os.path.join(folder_path, filename)
        large = cv2.imread(image_path)
        small = cv2.pyrDown(large)
        gray = cv2.cvtColor(small, cv2.COLOR_BGR2GRAY)
        kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3))
        grad = cv2.morphologyEx(gray, cv2.MORPH_GRADIENT, kernel)
        _, bw = cv2.threshold(grad, 0.0, 255.0, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

        kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (8, 5))

        connected = cv2.morphologyEx(bw, cv2.MORPH_CLOSE, kernel)

        contours, hierarchy = cv2.findContours(connected.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
        mask = np.zeros(bw.shape, dtype=np.uint8)

        valid_contours = []

        for idx in range(len(contours)):
            x, y, w, h = cv2.boundingRect(contours[idx])
            mask[y:y + h, x:x + w] = 0
            cv2.drawContours(mask, contours, idx, (255, 255, 255), -1)
            r = float(cv2.countNonZero(mask[y:y + h, x:x + w])) / (w * h)
            if r > 0.1 and w > 1 and h > 1:
                x = 2 * x
                y = 2 * y
                w = 2 * w
                h = 2 * h
                cv2.rectangle(large, (x, y), (x + w - 1, y + h - 1), (0, 255, 0), 2)
                valid_contours.append(contours[idx])

        save_rectangles(image_path, valid_contours)

        # show image with contours rect
        # cv2.namedWindow('rects', cv2.WINDOW_NORMAL)  # Create a named window with normal size
        # cv2.imshow('rects', large)
        # cv2.waitKey()

cv2.destroyAllWindows()
