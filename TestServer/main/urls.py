from django.urls import path
from . import views

urlpatterns = [
    path('', views.EntryPointAPIView.as_view())
]