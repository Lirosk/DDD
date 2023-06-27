import asyncio
import base64
import traceback
from rest_framework.generics import GenericAPIView
from rest_framework.response import Response
from rest_framework import status
from PIL import Image, ImageDraw
import io
from . import serializers
from . import utils

loop = asyncio.new_event_loop()

class EntryPointAPIView(GenericAPIView):
    serializer_class = serializers.EntryPointSerializer

    def post(self, request):
        print('post')
        
        try:
            serializer = self.serializer_class(data=request.data)
            serializer.is_valid(raise_exception=True)

            source_image_bytes = base64.b64decode(serializer.validated_data['image_bytes'])
            dpiY = serializer.validated_data['dpiY']
            translate_to_language = serializer.validated_data['translate_to_language']

            texts_with_coords = utils.recognize_text_from_image(source_image_bytes)
            strings_with_coords = utils.separate_texts_into_strings(texts_with_coords)
            translated_strings = utils.translate_texts(list(map(lambda x: x[0], strings_with_coords)), translate_to_language)

            image_bytes = utils.remove_texts(source_image_bytes, strings_with_coords, translated_strings)
            image_bytes = utils.place_texts_into_image(source_image_bytes, image_bytes, dpiY, strings_with_coords, translated_strings)

            # image = Image.open(io.BytesIO(image_bytes))
            # draw = ImageDraw.Draw(image)
            # for text, *coords in strings_with_coords:
            #     draw.rectangle(coords, outline='blue')

            # with io.BytesIO() as ms:
            #     image.save(ms, "PNG")
            #     image_bytes = ms.getvalue()

            base64_image = base64.b64encode(image_bytes).decode('utf-8')

            response_data = {
                'image_bytes': base64_image,
                'recognized_texts': texts_with_coords,
                'translated_texts': translated_strings
            }

            return Response(data=response_data, status=status.HTTP_200_OK)
        except Exception as e:
            traceback.print_exc()
            return Response({}, 500)
