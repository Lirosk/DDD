from rest_framework import serializers

class EntryPointSerializer(serializers.Serializer):
    image_bytes = serializers.CharField()