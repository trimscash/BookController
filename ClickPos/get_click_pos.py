import os
import pygame
import sys
from PIL import ImageGrab
import json
import time
import sys
from tkinter import*

sys.argv
dirname=sys.argv[1]
print(dirname)

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

img = pygame.transform.rotozoom(screenshot_image, 0, screen_width / screenshot_size[0])

# 画像を画面に描画
screen.blit(img, (0, 0))

# 画面をアップデート
pygame.display.flip()


# python get_click_pos.py  -option
mouse_pos={"left":{},"right":{}}
setting_items=["left","right"]
click_count=0
r=20


pygame.draw.rect(screen, (255,0,0), (0,0,screen_width, screen_height), 10)

is_file = os.path.isfile(dirname)
if is_file:
 #json読み込み
  json_open = open(dirname, 'r')
  json_load = json.load(json_open)

  exist_r=10
  pygame.draw.ellipse(screen,(255,190,255),(json_load['left']['x']-exist_r,json_load['left']['y']-exist_r,2*exist_r,2*exist_r))
  pygame.draw.ellipse(screen,(255,190,255),(json_load['right']['x']-exist_r,json_load['right']['y']-exist_r,2*exist_r,2*exist_r))

font = pygame.font.Font(None, 40)
text = font.render("Click the back button", True, (255,255,255), (255,0,0))
screen.blit(text, (screen_width/2-190, 50))


# イベントループ
running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.MOUSEBUTTONUP:
            pos = pygame.mouse.get_pos()
            print(pos)
            x,y=pos
            mouse_pos[setting_items[click_count]]["x"]=x
            mouse_pos[setting_items[click_count]]["y"]=y

            if setting_items[click_count]=="left":
                pygame.draw.ellipse(screen,(255,0,0),(x-r,y-r,2*r,2*r))
            if setting_items[click_count]=="right":
                pygame.draw.ellipse(screen,(0,0,255),(x-r,y-r,2*r,2*r))

            pygame.draw.rect(screen, (0,0,255), (0,0,screen_width, screen_height), 10)
            
            text = font.render("Click the Forward button", True, (255,255,255), (0,0,255))
            screen.blit(text, (screen_width/2-190, 50)) 

            print(mouse_pos)
            click_count=click_count+1
            

            print(dirname)
            with open(dirname, mode='w') as f:
                json.dump(mouse_pos, f, indent=2)
            
            

        if click_count == 2:
            running = False

        if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE):
            running = False
    pygame.display.update()                                 # 画面を更新
    

time.sleep(0.8)
# Pygameの終了
pygame.quit()
sys.exit()

