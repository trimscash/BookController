import pygame
import sys
from PIL import ImageGrab

# スクリーン全体をキャプチャ
screenshot = ImageGrab.grab()
# screenshot.show()

# 初期化
pygame.init()

screen_info = pygame.display.Info() 

# 画面サイズを取得
screen_width = screen_info.current_w 
screen_height = screen_info.current_h

# 画面の初期化
#screen = pygame.display.set_mode((0, 0), pygame.FULLSCREEN)
screen = pygame.display.set_mode((screen_width, screen_height), pygame.SCALED)

# キャプチャした画像を保存
screenshot_data = screenshot.tobytes()
screenshot_size = screenshot.size

screenshot_image= pygame.image.fromstring(screenshot_data, screenshot_size, "RGB")


# 画像を画面に描画
screen.blit(screenshot_image, (0, 0))

# 画面をアップデート
pygame.display.flip()

# イベントループ
running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
            running = False

# Pygameの終了
pygame.quit()
sys.exit()

