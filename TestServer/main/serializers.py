from rest_framework import serializers

class EntryPointSerializer(serializers.Serializer):
    image_bytes = serializers.CharField()
    dpiY = serializers.FloatField()
    translate_to_language = serializers.CharField()