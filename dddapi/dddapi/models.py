from django.db import models

class Screenshot(models.Model):
    image = models.ImageField(upload_to='screenshots/')
    language = models.CharField(max_length=100)

class TextBlock(models.Model):
    screenshot = models.ForeignKey(Screenshot, on_delete=models.CASCADE, related_name='text_blocks')
    text = models.TextField()
    position_x = models.IntegerField()
    position_y = models.IntegerField()
    width = models.IntegerField()
    height = models.IntegerField()
    font = models.CharField(max_length=100)
    font_size = models.IntegerField()
    font_color = models.CharField(max_length=100)
