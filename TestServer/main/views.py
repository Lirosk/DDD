import asyncio
import base64
import traceback
from rest_framework.generics import GenericAPIView
from rest_framework.response import Response
from rest_framework import status
from PIL import Image
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

            image_bytes = base64.b64decode(serializer.validated_data['image_bytes'])

            texts_with_coords = utils.recognize_text_from_image(image_bytes)
            
            translated_texts = utils.translate_texts(list(map(lambda x: x[0], texts_with_coords)), 'ru')
            image_bytes = utils.remove_texts(image_bytes, texts_with_coords, translated_texts)
            image_bytes = utils.place_texts_into_image(image_bytes, texts_with_coords, translated_texts)

            base64_image = base64.b64encode(image_bytes).decode('utf-8')

            response_data = {
                'image_bytes': base64_image,
                'recognized_texts': texts_with_coords,
                'translated_texts': translated_texts
            }

            return Response(data=response_data, status=status.HTTP_200_OK)
        except Exception as e:
            traceback.print_exc()
            return Response({}, 500)
