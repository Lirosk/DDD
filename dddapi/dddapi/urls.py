from django.contrib import admin
from django.urls import path
from . import views

urlpatterns = [
    path('admin/', admin.site.urls),
    path('upload-screenshot/', views.upload_screenshot, name='upload-screenshot'),
    path('add-text-block/', views.add_text_block, name='add-text-block'),
    path('translate-text/', views.translate_text, name='translate-text'),
]
