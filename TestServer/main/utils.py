import base64
import cv2
import numpy as np
import typing
from googletrans import Translator 
from google.cloud import vision_v1 as vision
from google.cloud.vision_v1 import types


translator = Translator()
client = vision.ImageAnnotatorClient()

def read_text_from_image(image_bytes: bytes) -> typing.List[typing.Tuple[str, int, int]]:
    response = client.annotate_image({
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