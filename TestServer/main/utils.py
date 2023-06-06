import base64
import cv2
import numpy as np
import typing

def remove_texts(image_bytes: bytes, texts_with_coords: typing.List[typing.Tuple[str, int, int]]) -> bytes:
    for text_with_coords in texts_with_coords:
        image_bytes = remove_text(image_bytes, text_with_coords)
    
    return image_bytes


def remove_text(image_bytes: bytes, text_with_coords: typing.Tuple[str, int, int]) -> bytes:
    nparr = np.frombuffer(image_bytes, np.uint8)

    image = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    top_left = text_with_coords[1]
    bottom_right = text_with_coords[2]

    mask = np.zeros(image.shape[:2], dtype=np.uint8)
    cv2.rectangle(mask, top_left, bottom_right, (255), cv2.FILLED)

    result = cv2.inpaint(image, mask, 3, cv2.INPAINT_TELEA)

    retval, buffer = cv2.imencode('.png', result)
    result_bytes = buffer.tobytes()

    return result_bytes