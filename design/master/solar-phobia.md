SOLAR PHOBIA - MASTER GAME DESIGN DOCUMENT (V5.0 - BẢN MỞ RỘNG CHI TIẾT)

Dự án: Conduit Summer Game Jam (1 Tháng) & Định hướng phát triển Full Release.
Theme: "Opposites Attract" (Hai thái cực hút nhau - Sự tĩnh lặng ngột ngạt vs. Tốc độ cuồng nộ, Hy vọng giả tạo vs. Tuyệt vọng tát ngã).
Thể loại: 2D Tactile Ritual Simulation (Mô phỏng nghi thức tương tác vật lý) x 2D Precision Platformer (Đi cảnh độ khó cao đòi hỏi sự chính xác tuyệt đối). Hai thể loại này cùng tồn tại trong một môi trường 2D nhưng khác biệt hoàn toàn về cấu trúc không gian, nhịp độ và trọng tâm cảm xúc.
Bối cảnh: Folk Horror (Kinh dị dân gian) - Cảm hứng sâu sắc từ văn hóa địa chất Đảo Lý Sơn và tín ngưỡng thờ Thần Nam Hải của ngư dân Việt Nam.
Engine đề xuất: Unity 2D (Sử dụng URP cho hệ thống ánh sáng 2D) / Godot 2D (Tận dụng sức mạnh của hệ thống Node và Physics 2D nội tại).

PHẦN 1: TỔNG QUAN, TRIẾT LÝ THIẾT KẾ & LORE CỐT LÕI

Solar Phobia không đơn thuần là một tựa game kinh dị sinh tồn; nó là một bài kiểm tra tâm lý tàn nhẫn về "Hy vọng vô vọng" (Hopeless Hope). Trò chơi xóa bỏ sự bất đồng giữa cốt truyện và lối chơi (Ludonarrative Dissonance) bằng cách biến mọi hành động đạo đức của người chơi thành các biến số toán học quyết định sự sống còn.

1.1. Nguồn gốc Tội lỗi & Trách nhiệm đứt gãy

Sự kiện nền: Trong tín ngưỡng ven biển, khi Cá Ông (Cá Voi) lụy bờ, ngài mang theo sự linh thiêng và phước lành. Người đầu tiên phát hiện xác ngài nghiễm nhiên mang trọng trách "Con trưởng" - phải đội khăn tang, mặc áo xô gai và tổ chức tang lễ ròng rã 3 năm ròng để báo đáp ân đức vị thần bảo hộ đại dương.

Tội lỗi của Tú: Tú vô tình phát hiện xác Cá Ông khổng lồ trôi dạt trong một đêm siêu bão. Nhưng trước áp lực tâm linh kinh hoàng và gánh nặng của hàng vạn sinh linh biển cả đè nặng lên xác vị thần, sự hèn nhát đã đánh gục Tú. Cậu hất bỏ dải Khăn Tang và quay lưng bỏ trốn.

Bản án Luyện Ngục: Lời thề bị bẻ gãy kéo theo sự thịnh nộ của đại dương. Trái với cái chết thông thường, Tú bị đày vào Cõi Trung Ấm (Bardo) - một Lõi Luyện Ngục méo mó mô phỏng lại chính bãi biển Lý Sơn. Tại đây, Tú bị ép phải làm một "Từ Trượng" (Người giữ lăng) mạt hạng, phải dùng chính đôi bàn tay đã từng vứt bỏ trách nhiệm để xoa dịu những oan hồn mà cậu từng chối bỏ.

1.2. Mặt Trời Rỗng (The Hollow Sun) - Con mắt của Karma

Không có mặt trời tự nhiên nào ở Cõi Trung Ấm. Thay vào đó là một thực thể phán xét khổng lồ mang họa tiết Trống Đồng rỉ sét lơ lửng trên bầu trời. Chiếc lỗ đen ở giữa khối cầu không ngừng xoáy sâu, tượng trưng cho hốc mắt trống rỗng, uất hận của xác Cá Ông lụy bờ.

Tác động vật lý & Tâm linh: Nó không tỏa ra nhiệt năng thông thường mà tỏa ra sức nóng của "Nghiệp" (Karma). Ánh sáng của nó là một dạng bức xạ tâm linh đặc quánh, thiêu rụi mọi sinh khí, làm bốc hơi độ ẩm và lập tức biến những linh hồn mang tội chạm vào nó thành một đống tro tàn trong sự đau đớn tột cùng.

PHẦN 2: PHASE BAN NGÀY - THE 1D X-AXIS CRUNCH (Trạng thái Tĩnh & Áp lực Đạo đức)

Để tối ưu hóa khối lượng công việc cho Team Art (tránh phải vẽ nhân vật 4 hướng) và đồng thời tối đa hóa cảm giác ngột ngạt (Claustrophobia), Phase Ban Ngày hoàn toàn sử dụng Góc nhìn 2D Side-scrolling Cận Cảnh (Macro View). Camera khóa chặt vào sạp hàng nhỏ bé của Tú.

2.1. Quản lý Không gian Trục Ngang (1D Spatial Management)

Cơ chế cốt lõi - Bức tường Ánh sáng: Nhân vật chỉ có thể di chuyển trên một đường thẳng duy nhất (Trục X). Bóng râm an toàn không phải là một vệt đen tĩnh dưới đất. Nó được định hình bởi Hai bức tường ánh sáng rực rỡ và chết chóc (từ Mặt Trời Rỗng) đổ xuống từ hai mép màn hình. Hai bức tường này bị bóp méo bởi hiệu ứng Heat Distortion và từ từ ép dần vào giữa theo thời gian thực một cách vô tình.

Sự chen chúc chân thực: Tú và 3 Hồn ma (Ông Văn, Em Linh, Anh Minh) đứng xếp thành một hàng ngang. Game áp dụng RigidBody2D cho các nhân vật, khiến họ có hitbox vật lý cứng. Họ không thể đi xuyên qua nhau. Sự chật chội tạo ra những tiếng va chạm xột xoạt của vải vóc và xác thịt khô héo. Khi hai bức tường nắng ép lại, tổng chiều rộng không gian sẽ không đủ cho 4 người.

Hành động ép buộc (The Core Mechanics):

[Swap - Đổi chỗ]: Tú không thể nhảy qua đầu hồn ma. Để tiếp cận một hồn ma đang bị kẹt ở ngoài rìa nắng, Tú phải tiến lại gần, tóm lấy áo họ và lộn vòng đổi chỗ (Swap animation kéo dài 0.5s), đưa linh hồn đó vào sâu bên trong vùng an toàn. Điều này đòi hỏi sự tính toán vị trí liên tục.

[Shove - Đẩy tàn nhẫn]: Khi không gian cạn kiệt ở cuối ngày, sự chen lấn đạt đỉnh điểm. Người chơi BẮT BUỘC phải dùng hành động xô đẩy (Shove). Màn hình rung chuyển (Screen shake), âm thanh va đập vang lên, và linh hồn thứ 3 bị văng ra khỏi màn hình, phơi mình dưới Mặt Trời Rỗng, bốc cháy thành tro với tiếng thét xé lòng.

2.2. Tiến trình 5 Phút (The Impossible Budget)

Game sử dụng chiến thuật thao túng tâm lý: Gieo rắc ảo tưởng rằng "Nếu tôi làm đủ nhanh, tôi có thể cứu tất cả", nhưng tài nguyên và không gian được lập trình để luôn luôn thiếu hụt.

0:00 – 1:30 (Ổn định - Ảo giác an toàn): Không gian râm mát rộng rãi đủ chỗ cho 4 người (Tú + 3 Ma). Người chơi thong thả thực hiện Minigame thủ công (Tactile Minigames) để kiếm điểm Hương Hỏa (Spirit Essence). Các thao tác đòi hỏi cảm giác vật lý: kéo chuột mồi diêm cần ma sát, giữ phím nghiêng nồi cháo phải cảm nhận được độ trễ của chất lỏng.

1:30 – 3:00 (Căng thẳng - Sự phản bội của môi trường): Bóng râm thu hẹp 30%. Light Interrupt xuất hiện: những tia nắng lọt qua khe hở của mái bạt rách hắt ngẫu nhiên xuống. Nếu tia nắng quét trúng tay Tú khi đang rót cháo, Tú sẽ bị bỏng, làm rơi bát cháo (Minigame thất bại, trừ điểm). Các hồn ma thiếu Hương Hỏa bắt đầu chuyển sang trạng thái hoảng loạn (Panic AI), chúng rên rỉ, đi lại cản đường Tú, phá vỡ trật tự xếp hàng.

3:00 – 4:30 (Khủng hoảng - Bất lực): Chỉ còn đúng chỗ cho 3 người đứng sát rạt vai nhau. Âm nhạc chuyển sang những nốt dây căng não. Người chơi phải điên cuồng bấm Swap để cứu vãn tình thế. Khoảnh khắc này ép người chơi phải cân nhắc giá trị của từng sinh mạng: Nên giữ lại bé gái (Em Linh) hay ông lão (Ông Văn)?

4:30 – 5:00 (Sụp đổ - Hậu quả): Hồn ma bị bỏ rơi cháy rụi, để lại một vệt đen trên cát. Hệ thống lập tức ghi ID của ma này vào biến sacrificed_ghost_id. Sạp hàng vỡ nát dưới áp lực tâm linh. Đêm ập xuống ngay lập tức. Camera Zoom-out đột ngột, ném người chơi từ một căn phòng chật hẹp ra một không gian mở tăm tối.

PHẦN 3: PHASE BAN ĐÊM - CHI TIẾT CƠ CHẾ PLATFORMER (Trạng thái Động)

Đây là một màn rượt đuổi tốc độ cao trên không gian mở (Agoraphobia) tại bãi đá trầm tích Lý Sơn. Tú phải dùng mọi ký ức cơ bắp (Muscle Memory) và kỹ năng Parkour để sống sót trước sự truy sát của Boss Cá Ông và những bẫy rập sinh ra từ oán niệm ban ngày.

3.1. Bộ Điều Khiển Kỹ Năng (The Fluid Controller)

Cảm giác điều khiển (Game Feel) là sinh mệnh của phần này. Tú không bay lơ lửng, chuyển động phải có sức nặng, lực hấp dẫn cao (Weighty Gravity) mang âm hưởng của Celeste hay Hollow Knight. Công cụ duy nhất để sống sót là Dải Khăn Tang bị nguyền rủa trên đầu Tú.

Yêu cầu Lập trình Bắt buộc (Must-have Features):

Coyote Time (0.1 giây): Phép màu của platformer. Cho phép người chơi nhấn nhảy ngay cả khi Tú vừa chạy hụt chân ra khỏi mép đá vài frame, tạo cảm giác điều khiển vô cùng công bằng và mượt mà.

Jump Buffering (0.15 giây): Ghi nhớ lệnh nhảy nếu người chơi bấm quá sớm (trước khi Tú chạm đất), giúp nhân vật nảy lên ngay lập tức khi vừa tiếp đất.

Kỹ năng Chủ động (Quyết định bài toán Tiêu hao Nước mắm cốt):

Jump (Nhảy - Miễn phí): Bấm nhấp nhẹ để nhảy lùn, giữ phím để vút lên cao tối đa. Khi đạt đỉnh nhảy (Apex), áp dụng hệ số lơ lửng cực ngắn (hang-time 0.1s) để người chơi dễ căn chỉnh điểm rơi.

Wall-Slide & Jump (Miễn phí): Khi lao vào vách đá nham thạch, Tú tự động cào tay vào đá (tạo tia lửa VFX), trượt xuống từ từ và có thể bật nhảy ngược chiều để leo lên các vách núi hẹp.

Spirit Dash (Lướt tàn ảnh - Tiêu hao cực nặng: -5.0s): Bấm Shift để lướt vút đi trên không trung hoặc mặt đất, xé toạc không khí, để lại dải tàn ảnh (Trail Renderer) phía sau. Dùng để né đòn khẩn cấp hoặc vượt qua các khe hẹp chết người.

Swing (Đu dây Khăn tang - Tiêu hao: -2.0s): Click chuột vào các Lư hương lơ lửng. Dải khăn tang phóng ra quấn chặt điểm neo, áp dụng vật lý con lắc (Pendulum physics) quăng cơ thể Tú bay qua những hố sụt khổng lồ.

Glide (Tụ gió - Tiêu hao: -1.0s/giây): Giữ Space khi đang rơi. Khăn tang bung rộng như một chiếc dù mỏng manh, giảm trọng lực rơi xuống còn 30%, giúp Tú từ từ hạ cánh xuống các bệ đá hẹp.

3.2. Cấu trúc Level (Map & Spawn Director)

Parallax Background: Biển đen sâu thẳm, mây cuộn sóng, mang lại cảm giác rộng lớn nhưng chết chóc.

Địa hình (Midground): Sự kỳ vĩ của thiên nhiên được mô phỏng qua các bãi đá nham thạch sần sùi, Gành Đá Đĩa (các cột bazan lục giác lên xuống thất thường tạo thành các bậc thang tự nhiên), và Cổng Tò Vò khổng lồ chặn ngang tầm nhìn. Level không phải vẽ tay 100% mà được chia thành các "Chunks" (Khối) tự động spawn và ghép nối liền mạch dựa trên thuật toán hạt giống (Procedural Generation).

The Cover System (Hệ thống Chỗ nấp - Core Stealth): Dọc đường chạy có rải rác các Mộ Gió (tượng đất sét nặn hình nhân). Người chơi sử dụng phím S (Crouch) để ngồi thụp xuống phía sau các bức tượng này, thu nhỏ hitbox và trốn tránh ánh nhìn của quái vật.

3.3. Tương tác Boss: Cơn phẫn nộ "Searchlight" của Cá Ông

Cá Ông không phải là con quái vật chạy rầm rập sau lưng. Bộ xương khổng lồ (Leviathan Skeleton) của ngài bơi lờ mờ ở lớp Background xa tít, tạo ra áp lực vô hình bao trùm toàn bộ màn chơi. Ngài săn tìm Tú bằng cơ chế Tàng hình/Ánh sáng.

Cơ chế Tia Quét (The Gaze): Từ hốc mắt trống rỗng của bộ xương, một tia sáng hình nón màu Xanh Lục Đáy Biển (có kèm hiệu ứng quang sai màu viền) liên tục quét dọc theo địa hình, xuyên qua các lớp sương mù.

Đòn Trừng Phạt (The Strike): Nếu tia sáng quét qua và khóa mục tiêu lên người Tú lúc cậu đang chạy hớ hênh hoặc nhảy lơ lửng trên không (KHÔNG nấp sau Mộ Gió), mặt biển sẽ sôi sục. Cá Ông gầm lên một tiếng chát chúa và lập tức ném một tảng xương/đá khổng lồ với quỹ đạo vòng cung đập thẳng vào vị trí của Tú.

Hậu quả & Phục hồi: Bị đập trúng, Tú văng đi, màn hình nhòe đi vì đau đớn và mất ngay lập tức -20s thời gian sống sót. Mộ gió hiện tại bị tảng xương nghiền nát. Tuy nhiên, để tránh vòng lặp chết liên hoàn (Anti-stunlock), chính tảng xương khổng lồ vừa ném xuống sẽ cắm phập xuống lớp cát đen, tạo thành một chỗ nấp mới (dù có diện tích nhỏ hơn và khó nấp hơn) cho lần quét tiếp theo.

3.4. Hệ Quả Karma (Choice-Seeded Hazards)

Sự độc đáo của Level Design ban đêm là nó được "cấy ghép" bởi tội lỗi ban ngày. Map sẽ tự động sinh thêm các loại bẫy chết người dựa vào biến sacrificed_ghost_id:

Nếu bỏ rơi Ông Văn -> Lưới Máu: Những dải lưới đánh cá mục nát bám đầy hàu sắc nhọn vắt ngang trên không. Chạm vào không gây chết ngay nhưng mất -5.0s và bị nguyền rủa làm chậm (Slow 50% di chuyển trong 3 giây). Trong game tốc độ cao, bị làm chậm đồng nghĩa với việc trở thành mồi ngon cho tia sáng Cá Ông. Buộc người chơi phải Swing hoặc Glide khéo léo để vòng qua.

Nếu bỏ rơi Em Linh -> Vũng Nước Tử Thần: Dưới các khe nứt địa chất nước dâng lên thứ màu lam ngọc phát sáng. Ngâm mình xuống đó sẽ bị rút máu liên tục với sát thương DoT (Damage over Time: -3s/giây), kèm tiếng trẻ con khóc vọng lên từ đáy nước. Ép người chơi phải bám chặt các vách đá trên cao.

Nếu bỏ rơi Anh Minh -> Bệ Đá Ảo Ảnh: Những bệ đá tàn tro bốc khói đen. Chúng trông rất vững chãi nhưng ngay khi Tú giẫm lên, bệ đá sẽ rạn nứt và vỡ tung rơi xuống vực sâu sau đúng 0.2 giây. Ép người chơi phải duy trì Flow (dòng chảy hành động) không được phép dừng lại, phải Dash bật đi ngay lập tức.

3.5. Sự Đánh Đổi Lòng Tham (Ngọc Cốt - The Cursed Relic)

Phần thưởng: Nằm lấp lánh ở các Mộ Oán nứt nẻ. Nhặt Ngọc Cốt là điều kiện tiên quyết để tăng tiến trình mở khóa "True Ending" và giải thoát hoàn toàn Lời nguyền.

Rủi ro bóp nghẹt: Ngay khi chạm vào Ngọc Cốt, Tú dính trạng thái "Ngộ độc ảo giác". Màn hình méo mó bởi hiệu ứng Chromatic Aberration mạnh. Ác mộng thực sự là tốc độ tụt thanh thời gian sống sót tự nhiên sẽ tăng theo cấp số nhân (x2, x3).

Luật Spawn (Anti-Exploit): Để chống lại việc người chơi lách luật "nhặt Ngọc xong nhảy ngay qua cửa màn", Ngọc Cốt TUYỆT ĐỐI KHÔNG được spawn ở 30% chặng đường cuối của Chunk. Nó ép người chơi phải chịu đựng hiệu ứng ảo giác và thực hiện Parkour hoàn hảo qua một đoạn đường dài đầy đau đớn.

PHẦN 4: HỆ THỐNG SINH TỒN CỐT LÕI & HUD-LESS DESIGN

Trò chơi áp dụng triết lý Thiết kế Tối giản (Minimalist UI), hoàn toàn KHÔNG CÓ giao diện thanh máu, minimap hay thanh mana cản trở tầm nhìn.
Chỉ số sống còn duy nhất chạy ngầm trong code là Ward Timer (Nước mắm cốt / Thời gian sống). Khi đồng hồ này đếm ngược về 0, Dải Khăn tang sẽ tự động siết chặt cổ Tú, kéo ngược cậu chìm sâu xuống đáy đại dương, báo hiệu Game Over.

4.1. Công thức Toán học Định hình Độ Khó

Sự sống ban đêm được mua bằng sự chăm chỉ ban ngày.

Khởi tạo Đêm (Initial Ward): Base (10s) + (Hương Hỏa tích lũy từ Minigame) - (Phạt làm đổ đồ cúng).
(Lưu ý cực kỳ quan trọng cho Coder: Base chỉ có vỏn vẹn 10 giây. Nó ngăn chặn lối chơi Speedrunner bất cần. Nếu bạn lười biếng, đẩy cả 3 hồn ma ban ngày để vào màn đêm cho nhanh, bạn chỉ có 10 giây và sẽ chết gục giữa đường mà không có cách nào cứu vãn).

Tốc độ Cạn kiệt tự nhiên (Passive Drain): Drain = 1.0/giây * (1 + Số Ngọc Cốt đang cầm trong người). Sự cạn kiệt này luôn diễn ra, là một áp lực thời gian (Time-attack) vô hình.

4.2. Hệ thống Giao Tiếp Cảm Giác (Sensory Readability)

Thay vì nhìn số, người chơi "CẢM NHẬN" cái chết đang đến gần qua việc hệ thống tháo gỡ từng lớp giác quan:

Tier 1 (Ward > 75% - Khỏe mạnh): Hình ảnh trong trẻo, sắc nét. Nhạc nền (BGM) nhịp độ dồn dập, tiếng sóng vỗ mạnh mẽ hùng hồn, truyền cảm hứng tốc độ.

Tier 2 (Ward < 75% - Chớm mệt mỏi): Viền màn hình từ từ xuất hiện các mảng mờ đen (Vignette). Âm thanh môi trường và nhạc nền bắt đầu bị bộ lọc tần số thấp (Lowpass filter) bóp nghẹt, như thể tai Tú đang bị nước biển tràn vào.

Tier 3 (Ward < 50% - Gánh nặng thể xác): Âm thanh tiếng tim đập thình thịch và tiếng thở dốc nặng nề của Tú đè lên nhạc nền. Phạt Cơ học thực tế: Cooldown của kỹ năng Dash bị ép thêm độ trễ (delay) đi 0.1s. Tú thực sự phản ứng chậm lại, tạo cảm giác nặng nề tuyệt vọng.

Tier 4 (Ward < 25% - Cơn hoảng loạn): Toàn bộ màn hình nhòe màu liên tục (Chromatic Aberration intensity cao). Controller rung liên hồi. Tiếng ma quỷ thì thầm khóc lóc, nguyền rủa vang dội đan chéo hai bên tai nghe (Stereo panning).

Tier 5 (Ward < 10s - Death Spiral): Tiếng ù tai chói lói ác mộng (Tinnitus) lấn át mọi thứ. Hiệu ứng Tầm nhìn ống (Tunnel Vision) thu lại thành một lỗ đen che khuất 80% màn hình, chỉ để lại một đốm sáng le lói. Bóng dáng Boss Cá Ông rống lên từ phía sau báo hiệu kết cục đã điểm.