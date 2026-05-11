# **SOLAR PHOBIA \- MASTER GAME DESIGN DOCUMENT (V5.0)**

**Dự án:** Conduit Summer Game Jam (1 Tháng) & Định hướng phát triển Full Release.  
**Theme:** "Opposites Attract" (Hai thái cực hút nhau \- Sự tĩnh lặng ngột ngạt vs. Tốc độ cuồng nộ).  
**Thể loại:** 2D Tactile Ritual Simulation x 2D Precision Platformer.  
**Bối cảnh:** Folk Horror (Kinh dị dân gian) \- Cảm hứng từ văn hóa Đảo Lý Sơn và tín ngưỡng thờ Thần Nam Hải.

## **PHẦN 1: TỔNG QUAN & LORE CỐT LÕI**

*Solar Phobia* là một thí nghiệm tâm lý về **"Hy vọng vô vọng" (Hopeless Hope)**.

* **Nguồn gốc tội lỗi:** Tú vô tình phát hiện Cá Ông lụy bờ nhưng hèn nhát vứt bỏ Khăn Tang, trốn tránh trách nhiệm làm "Con trưởng" để tang 3 năm. Lời thề bị bẻ gãy kéo theo sự thịnh nộ của đại dương.  
* **Mặt Trời Rỗng (The Hollow Sun):** Thực thể phán xét thay thế mặt trời tự nhiên. Chiếc lỗ đen ở giữa tượng trưng cho hốc mắt rỗng của Cá Ông. Nó tỏa ra sức nóng của "Nghiệp" (Karma), thiêu rụi mọi sinh khí thành tro bụi.

## **PHẦN 2: PHASE BAN NGÀY \- THE 1D X-AXIS CRUNCH**

Ban Ngày hoàn toàn sử dụng góc nhìn **2D Side-scrolling Cận Cảnh (Macro View)** để tối ưu cho Team Art và tạo cảm giác ngột ngạt.

### **2.1. Quản lý Không gian Trục Ngang**

Nhân vật di chuyển trên một đường thẳng (Trục X). Bóng râm được định hình bởi hai bức tường ánh sáng ép dần vào giữa. Tú phải sử dụng kỹ năng **Swap** (Đổi chỗ) để bảo vệ linh hồn và **Shove** (Đẩy) khi không gian cạn kiệt.

### **2.2. Timeline 5 Phút (The Impossible Budget)**

Tiến trình được chia làm 4 giai đoạn leo thang áp lực:

* **0:00 – 1:30 (Ổn định):** Thực hiện Ritual thong thả (Quẹt diêm, Rót cháo, Vẩy nước).  
* **1:30 – 3:00 (Căng thẳng):** Bóng râm thu hẹp 30%. Xuất hiện Light Interrupt làm hỏng Ritual.  
* **3:00 – 4:30 (Khủng hoảng):** Không gian chỉ đủ cho 3 người. Ép buộc phải ra quyết định hy sinh.  
* **4:30 – 5:00 (Sụp đổ):** Hồn ma bị bỏ rơi cháy rụi. Camera Zoom-out chuyển Phase Đêm.

## **PHẦN 3: PHASE BAN ĐÊM \- CHI TIẾT CƠ CHẾ PLATFORMER**

### **3.1. Bộ Điều Khiển Kỹ Năng (The Fluid Controller)**

Tú sử dụng Dải Khăn Tang để di chuyển linh hoạt:

* **Spirit Dash:** Lướt nhanh né đòn (Tiêu hao \-5.0s).  
* **Swing:** Quăng khăn quấn neo để đu người (Tiêu hao \-2.0s).  
* **Glide:** Bung khăn lướt gió (Tiêu hao \-1.0s/giây).  
* **Yêu cầu:** Coyote Time (0.1s) và Jump Buffering (0.15s).

### **3.2. Tương tác Boss Cá Ông**

Cá Ông săn lùng Tú bằng tia quét **Searchlight Xanh Lục**. Người chơi phải nấp sau các **Mộ Gió** dọc đường chạy. Nếu bị phát hiện, Cá Ông sẽ ném tảng xương lớn gây sát thương nặng (-20s).

## **PHẦN 4: HỆ THỐNG SINH TỒN & ĐIỀU KHIỂN**

### **4.1. Công thức Ward Timer**

Game không có UI, chỉ có thời gian sống (Nước mắm cốt):  
Initial\_Ward \= 10s (Base) \+ (Hương Hỏa tích lũy) \- Phạt lỗi

### **4.2. Sơ đồ phím tắt (Input Mapping)**

| Hành động | Ban Ngày | Ban Đêm |
| :---- | :---- | :---- |
| Di chuyển | A, D (Chậm) | A, D (Sprint) |
| Tương tác | Click Trái (Ritual) | Swing (Click Trái) |
| Hành động đặc biệt | Space (Swap) / F (Shove) | Shift (Dash) / S (Crouch) |

## **PHẦN 5: YÊU CẦU MỸ THUẬT (ART BIBLE)**

* **Phong cách:** Dark Fantasy, Folk Horror, tương phản sáng tối gắt.  
* **Bảng màu:** Vàng Cam cháy (Ngày) vs. Xanh Biển thẳm (Đêm).  
* **Cá Ông:** Module rách nát tách rời (Sọ bám hàu, hàm dưới lởm chởm).