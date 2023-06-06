import base64
import traceback
from rest_framework.generics import GenericAPIView
from rest_framework.response import Response
from rest_framework import status
from PIL import Image
import io
from .OCR import OCR
from . import serializers
from . import utils

class EntryPointAPIView(GenericAPIView):
    serializer_class = serializers.EntryPointSerializer

    def get(self, request):
        return Response('ASAFEEFW')
    

    def post(self, request):
        print('post')
        
        try:
            serializer = self.serializer_class(data=request.data)
            serializer.is_valid(raise_exception=True)

            image_bytes = base64.b64decode(serializer.validated_data['image_bytes'])

            text_with_coords = OCR.read_text_from_image(image_bytes)

            result_image_bytes = utils.remove_texts(image_bytes, text_with_coords)

            base64_image = base64.b64encode(result_image_bytes).decode('utf-8')

            response_data = {
                'image_bytes': base64_image,
                'recognized_texts': text_with_coords
            }

            return Response(data=response_data, status=status.HTTP_200_OK)
        except Exception as e:
            traceback.print_exc()
            return Response({}, 500)