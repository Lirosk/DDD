from rest_framework import serializers
from .models import Screenshot, TextBlock

class TextBlockSerializer(serializers.ModelSerializer):
    class Meta:
        model = TextBlock
        fields = '__all__'

class ScreenshotSerializer(serializers.ModelSerializer):
    text_blocks = TextBlockSerializer(many=True, read_only=True)

    class Meta:
        model = Screenshot
        fields = '__all__'
