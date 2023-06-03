from django.shortcuts import get_object_or_404
from rest_framework.decorators import api_view
from rest_framework.response import Response
from rest_framework import status
from .models import Screenshot, TextBlock
from .serializers import ScreenshotSerializer, TextBlockSerializer

@api_view(['POST'])
def upload_screenshot(request):
    serializer = ScreenshotSerializer(data=request.data)
    if serializer.is_valid():
        screenshot = serializer.save()
        # Дополнительная логика по сохранению снимка экрана
        return Response({'message': 'Снимок экрана успешно загружен.'})
    return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

@api_view(['POST'])
def add_text_block(request):
    serializer = TextBlockSerializer(data=request.data)
    if serializer.is_valid():
        text_block = serializer.save()
        # Дополнительная логика по сохранению блока текста
        return Response({'message': 'Блок текста успешно добавлен.'})
    return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

@api_view(['POST'])
def translate_text(request):
    screenshot_id = request.data.get('screenshot_id')
    screenshot = get_object_or_404(Screenshot, id=screenshot_id)
    text_blocks = TextBlock.objects.filter(screenshot=screenshot)

    # Логика по переводу текста для каждого блока
    ...

    return Response({'message': 'Текст успешно переведен.'})
