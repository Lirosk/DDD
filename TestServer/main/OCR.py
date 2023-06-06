import os
import typing
from google.cloud import vision_v1 as vision

os.environ['GOOGLE_APPLICATION_CREDENTIALS'] = r'C:/Users/kiril/AppData/Roaming/GoogleCloud/application_default_credentials.json'

client = vision.ImageAnnotatorClient()

class OCR:
    @classmethod
    def read_text_from_image(cls, image_bytes: bytes) -> typing.List[typing.Tuple[str, int, int]]:
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
