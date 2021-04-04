-- phpMyAdmin SQL Dump
-- version 5.0.1
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 192.168.11.27
-- Thời gian đã tạo: Th3 31, 2021 lúc 08:34 AM
-- Phiên bản máy phục vụ: 10.4.13-MariaDB-1:10.4.13+maria~focal
-- Phiên bản PHP: 7.2.24-0ubuntu0.18.04.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `alta_crm_i9ch`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_group`
--

CREATE TABLE `hd_bank_group` (
  `group_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `group_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `group_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp() COMMENT 'DateTime.Now',
  `updated_at` datetime DEFAULT current_timestamp() COMMENT 'DateTime.Now'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_group`
--

INSERT INTO `hd_bank_group` (`group_id`, `group_name`, `group_code`, `created_at`, `updated_at`) VALUES
('95609553-a243-434e-a1ea-a9fd64f3861a', '                                  ', '                                 ', '2021-03-18 03:43:13', NULL),
('9bb07273-ef63-4a17-aebf-2751efc7b1d8', 'Group_ VIP', 'group_vip', '2021-02-03 03:51:59', '2021-03-25 04:14:46'),
('c71fe372-1311-44bb-8a7c-6c894a0043e7', 'Nhóm nhân viên', 'group_staff', '2020-12-29 09:57:32', '2021-03-30 09:34:16'),
('customer', 'Nhóm Khách', 'group_guest', '2020-12-29 09:57:32', NULL),
('group_default', 'Group Default', 'group_default', '2021-01-07 02:34:18', NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_media`
--

CREATE TABLE `hd_bank_media` (
  `media_id` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `media_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `media_path` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `media_title` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `media_tag` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `media_description` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `media_created_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_meeting_schedule`
--

CREATE TABLE `hd_bank_meeting_schedule` (
  `meeting_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `meeting_title` text COLLATE utf8_unicode_ci NOT NULL,
  `meeting_content` text COLLATE utf8_unicode_ci NOT NULL,
  `meeting_time_start` datetime NOT NULL,
  `meeting_time_end` datetime NOT NULL,
  `user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_mode_authentication`
--

CREATE TABLE `hd_bank_mode_authentication` (
  `mode_authentication_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_authentication_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_mode_authentication`
--

INSERT INTO `hd_bank_mode_authentication` (`mode_authentication_id`, `mode_authentication_name`, `created_at`) VALUES
('Card_ID', 'Card ID', '2020-12-22 16:18:16'),
('Face_ID', 'Face ID', '2020-12-28 11:23:40'),
('Finger_Print', 'Finger Print', '2020-12-22 16:17:33');

--
-- Bẫy `hd_bank_mode_authentication`
--
DELIMITER $$
CREATE TRIGGER `tg_crm_delete_mode_authencation` BEFORE DELETE ON `hd_bank_mode_authentication` FOR EACH ROW if( old.mode_authentication_id = 'Card_ID' OR old.mode_authentication_id = 'Face_ID' OR old.mode_authentication_id = 'Finger_Print')
	THEN
    signal sqlstate '42000' set message_text = 		"mode defaul not delete";
END IF
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_ratings`
--

CREATE TABLE `hd_bank_ratings` (
  `rating_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `robot_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `rating_star` int(11) NOT NULL DEFAULT 0,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `user_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_name_display` text COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_robot`
--

CREATE TABLE `hd_bank_robot` (
  `robot_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `robot_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_robot`
--

INSERT INTO `hd_bank_robot` (`robot_id`, `robot_name`, `created_at`) VALUES
('9a8031b1-9c37-4af3-84e3-0b01cdfae31a', 'Robot', '2020-12-25 08:52:12'),
('c48fc3f3-adf9-476b-b5e1-480becd2571d', 'Optimus Prime', '2021-02-02 06:54:35');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_schedules`
--

CREATE TABLE `hd_bank_schedules` (
  `schedule_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `schedule_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `schedule_description` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `ticket_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tag_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `schedule_repeat_type` tinyint(4) NOT NULL DEFAULT 0 COMMENT '0 : No Repeat\r\n1 : Daily\r\n2 : Weekly\r\n3 : Monthly\r\n4 : Yearly',
  `schedule_date_start` datetime NOT NULL,
  `schedule_date_end` datetime NOT NULL,
  `schedule_time_start` time NOT NULL,
  `schedule_time_end` time NOT NULL,
  `schedule_value` text COLLATE utf8_unicode_ci DEFAULT NULL,
  `schedule_created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_tags`
--

CREATE TABLE `hd_bank_tags` (
  `tag_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tag_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tag_comment` text COLLATE utf8_unicode_ci DEFAULT NULL,
  `tag_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `tag_status` int(11) NOT NULL DEFAULT 0 COMMENT 'Trạng thái tồn tại ở AC: 0 = chưa, 1 = rồi',
  `ticket_type_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `tag_repeat_type` int(11) DEFAULT NULL,
  `tag_date_start` datetime DEFAULT NULL,
  `tag_date_end` datetime DEFAULT NULL,
  `tag_repeat_value` text COLLATE utf8_unicode_ci DEFAULT NULL,
  `time_start` time DEFAULT NULL,
  `time_end` time DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp() COMMENT 'DateTime.Now',
  `updated_at` datetime DEFAULT current_timestamp() COMMENT 'DateTime.Now',
  `repository_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_tags`
--

INSERT INTO `hd_bank_tags` (`tag_id`, `tag_name`, `tag_comment`, `tag_code`, `tag_status`, `ticket_type_id`, `tag_repeat_type`, `tag_date_start`, `tag_date_end`, `tag_repeat_value`, `time_start`, `time_end`, `created_at`, `updated_at`, `repository_id`) VALUES
('7e2e6451-2641-44c7-af51-752cf97476c2', 'backend_Test', NULL, 'default_tag', 0, '14341913-d0e6-43df-aaf9-ec5e3188e898', 1, '2021-03-28 21:47:15', '2021-04-23 21:47:15', '[]', NULL, NULL, '2021-03-18 03:43:13', '2021-03-30 11:02:16', 'b50e47b4-4ca5-400e-b776-63d96c2a79f7');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_ticket`
--

CREATE TABLE `hd_bank_ticket` (
  `ticket_type_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_description` text COLLATE utf8_unicode_ci DEFAULT NULL,
  `devices` text COLLATE utf8_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_ticket`
--

INSERT INTO `hd_bank_ticket` (`ticket_type_id`, `ticket_type_name`, `ticket_type_description`, `devices`, `created_at`, `updated_at`) VALUES
('14341913-d0e6-43df-aaf9-ec5e3188e898', 'ticket_new', 'ticket_new', '[\"037ae40e-0641-4d8f-83be-43f741c0666d\",\"4511bb7a-9f91-441e-865c-79fcc9a6dee9\",\"b343bfaa-2bb1-4bd5-8c5f-45f5a29bbdb1\"]', '2021-03-18 03:43:13', NULL),
('21e9fde0-f980-44e8-8840-8e4b845c9c45', 'test hoa', 'test hoa', '[\"4511bb7a-9f91-441e-865c-79fcc9a6dee9\"]', '2021-03-16 11:34:21', NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_users`
--

CREATE TABLE `hd_bank_users` (
  `user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `group_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_code_active` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_first_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_last_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_gender` tinyint(2) DEFAULT NULL COMMENT '0: là nam, 1 là nữ',
  `user_image` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_phone` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_email` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_address` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `face_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `card_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fingerprint_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_status` tinyint(2) NOT NULL DEFAULT 1 COMMENT '0 : lock, 1 : unlock',
  `user_created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `user_updated_at` datetime DEFAULT NULL,
  `user_tag_ids` longtext COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_users`
--

INSERT INTO `hd_bank_users` (`user_id`, `group_id`, `user_code_active`, `user_first_name`, `user_last_name`, `user_gender`, `user_image`, `user_phone`, `user_email`, `user_address`, `face_id`, `card_id`, `fingerprint_id`, `user_status`, `user_created_at`, `user_updated_at`, `user_tag_ids`) VALUES
('05a3174b-f4bf-4d0c-b848-cd79884f3046', '9bb07273-ef63-4a17-aebf-2751efc7b1d8', '368203', 'test', 'test', 0, '', '0771234567', '123@alta.com.vn', '123@@', NULL, NULL, NULL, 1, '2021-03-23 09:17:19', NULL, '[\"7e2e6451-2641-44c7-af51-752cf97476c2\"]'),
('3beda24e-04ef-4b9b-868e-834206695a65', 'c71fe372-1311-44bb-8a7c-6c894a0043e7', '631274', 'test test', 'nguyễn', 1, '24-03-2021/637521576608054461_408ac5a5102e42678de2827089cf177b.jpg', '0771235897', 'phuc@alta.com.vn', '123 Âu Cơ', NULL, NULL, NULL, 1, '2021-03-22 09:20:58', NULL, '[\"7e2e6451-2641-44c7-af51-752cf97476c2\"]'),
('a24cdf0e-8f66-4b70-81bc-65c6f4faa37f', 'c71fe372-1311-44bb-8a7c-6c894a0043e7', '501122', 'test hoa', 'nguyen', 0, '17-03-2021/637515747968303916_child-happy-face.jpg', '0905907362', 'hoa.nlx@alta.com.vb', 'test', NULL, NULL, NULL, 1, '2021-03-17 10:46:36', NULL, '[\"d2957cb6-5489-47fe-bd9b-4feb32571772\"]'),
('a8975049-9419-4b7a-9b8e-e72cad913c3d', 'group_default', '373082', 'Lê', 'Thị Luyên', 1, '29-03-2021/637525902834419294_637521577833650516_408ac5a5102e42678de2827089cf177b.jpg', '0379696398', 'asdasd@gmail.com', 'w', NULL, NULL, NULL, 1, '2021-03-29 04:51:23', NULL, '[\"7e2e6451-2641-44c7-af51-752cf97476c2\"]');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_users_modes`
--

CREATE TABLE `hd_bank_users_modes` (
  `users_modes_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `users_modes_key_code` varchar(500) COLLATE utf8_unicode_ci NOT NULL,
  `users_modes_image` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `users_modes_created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `repository_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_users_modes`
--

INSERT INTO `hd_bank_users_modes` (`users_modes_id`, `user_id`, `mode_id`, `users_modes_key_code`, `users_modes_image`, `users_modes_created_at`, `repository_id`) VALUES
('2b3b7663-76eb-4fc8-850d-f79d70f4aeb3', 'a24cdf0e-8f66-4b70-81bc-65c6f4faa37f', 'Face_ID', 'c1442069-6c25-453a-b6c1-ed3df58d7ed7', '17-03-2021/637515747968303916_child-happy-face.jpg', '2021-03-17 03:46:43', 'df529083-126b-4eed-8888-f541f97ab458'),
('5c9a6e37-b43c-4eeb-8917-e8da40be10d8', 'a8975049-9419-4b7a-9b8e-e72cad913c3d', 'Face_ID', '7aa39409-cb77-4bd8-ae9c-42cb551a4a01', '29-03-2021/637525902834419294_637521577833650516_408ac5a5102e42678de2827089cf177b.jpg', '2021-03-28 21:51:26', 'b50e47b4-4ca5-400e-b776-63d96c2a79f7'),
('6edb0787-8b78-4948-8fa1-fbf0ad5ab982', '05a3174b-f4bf-4d0c-b848-cd79884f3046', 'Face_ID', '8f3f1255-0583-4998-9825-afae6b57d9dd', '24-03-2021/637521577833650516_408ac5a5102e42678de2827089cf177b.jpg', '2021-03-23 21:43:03', '5466b14e-fcbd-43bf-8231-ef76de9ada80'),
('9a45a56f-8a4d-4cb1-a5a2-7754606e5b4c', '3beda24e-04ef-4b9b-868e-834206695a65', 'Face_ID', '9a45a56f-8a4d-4cb1-a5a2-7754606e5b4c', '24-03-2021/637521576608054461_408ac5a5102e42678de2827089cf177b.jpg', '2021-03-23 21:41:04', 'df529083-126b-4eed-8888-f541f97ab458');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_users_tag_modes`
--

CREATE TABLE `hd_bank_users_tag_modes` (
  `users_tag_mode_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tag_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_tag_mode_created_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hd_bank_users_tag_modes`
--

INSERT INTO `hd_bank_users_tag_modes` (`users_tag_mode_id`, `tag_id`, `user_id`, `mode_id`, `user_tag_mode_created_at`) VALUES
('1c07af80-0106-4d50-9d09-d613d93f6a87', 'd2957cb6-5489-47fe-bd9b-4feb32571772', 'a24cdf0e-8f66-4b70-81bc-65c6f4faa37f', 'Face_ID', '2021-03-22 08:22:32'),
('56f2e40a-7f27-427f-ab06-38e21766f645', '7e2e6451-2641-44c7-af51-752cf97476c2', '3beda24e-04ef-4b9b-868e-834206695a65', 'Face_ID', '2021-03-30 08:26:27'),
('7345ca32-52c5-4aa2-9ec9-392c80f35dd0', '7e2e6451-2641-44c7-af51-752cf97476c2', 'a8975049-9419-4b7a-9b8e-e72cad913c3d', 'Face_ID', '2021-03-29 04:51:26'),
('95abc586-888c-4562-9912-aa429c020704', '7e2e6451-2641-44c7-af51-752cf97476c2', '05a3174b-f4bf-4d0c-b848-cd79884f3046', 'Face_ID', '2021-03-30 08:07:34');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hd_bank_user_codes`
--

CREATE TABLE `hd_bank_user_codes` (
  `user_code_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_code_active` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_code_expire` datetime DEFAULT NULL,
  `user_code_created_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `hd_bank_group`
--
ALTER TABLE `hd_bank_group`
  ADD PRIMARY KEY (`group_id`),
  ADD UNIQUE KEY `group_code` (`group_code`);

--
-- Chỉ mục cho bảng `hd_bank_media`
--
ALTER TABLE `hd_bank_media`
  ADD PRIMARY KEY (`media_id`);

--
-- Chỉ mục cho bảng `hd_bank_meeting_schedule`
--
ALTER TABLE `hd_bank_meeting_schedule`
  ADD PRIMARY KEY (`meeting_id`),
  ADD KEY `fk_user_id` (`user_id`);

--
-- Chỉ mục cho bảng `hd_bank_mode_authentication`
--
ALTER TABLE `hd_bank_mode_authentication`
  ADD PRIMARY KEY (`mode_authentication_id`);

--
-- Chỉ mục cho bảng `hd_bank_ratings`
--
ALTER TABLE `hd_bank_ratings`
  ADD PRIMARY KEY (`rating_id`);

--
-- Chỉ mục cho bảng `hd_bank_robot`
--
ALTER TABLE `hd_bank_robot`
  ADD PRIMARY KEY (`robot_id`);

--
-- Chỉ mục cho bảng `hd_bank_schedules`
--
ALTER TABLE `hd_bank_schedules`
  ADD PRIMARY KEY (`schedule_id`);

--
-- Chỉ mục cho bảng `hd_bank_tags`
--
ALTER TABLE `hd_bank_tags`
  ADD PRIMARY KEY (`tag_id`),
  ADD KEY `fk_ticket` (`ticket_type_id`);

--
-- Chỉ mục cho bảng `hd_bank_ticket`
--
ALTER TABLE `hd_bank_ticket`
  ADD PRIMARY KEY (`ticket_type_id`);

--
-- Chỉ mục cho bảng `hd_bank_users`
--
ALTER TABLE `hd_bank_users`
  ADD PRIMARY KEY (`user_id`),
  ADD UNIQUE KEY `face_id` (`face_id`),
  ADD UNIQUE KEY `card_id` (`card_id`),
  ADD UNIQUE KEY `fingerprint_id` (`fingerprint_id`),
  ADD KEY `FK_user_group` (`group_id`);

--
-- Chỉ mục cho bảng `hd_bank_users_modes`
--
ALTER TABLE `hd_bank_users_modes`
  ADD PRIMARY KEY (`users_modes_id`),
  ADD KEY `fk_user_user_mode` (`user_id`);

--
-- Chỉ mục cho bảng `hd_bank_users_tag_modes`
--
ALTER TABLE `hd_bank_users_tag_modes`
  ADD PRIMARY KEY (`users_tag_mode_id`),
  ADD KEY `fk_user_user_tag_mode` (`user_id`);

--
-- Chỉ mục cho bảng `hd_bank_user_codes`
--
ALTER TABLE `hd_bank_user_codes`
  ADD PRIMARY KEY (`user_code_id`),
  ADD KEY `fK_user_user_codes` (`user_id`);

--
-- Các ràng buộc cho các bảng đã đổ
--

--
-- Các ràng buộc cho bảng `hd_bank_meeting_schedule`
--
ALTER TABLE `hd_bank_meeting_schedule`
  ADD CONSTRAINT `fk_user_id` FOREIGN KEY (`user_id`) REFERENCES `hd_bank_users` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Các ràng buộc cho bảng `hd_bank_tags`
--
ALTER TABLE `hd_bank_tags`
  ADD CONSTRAINT `fk_ticket` FOREIGN KEY (`ticket_type_id`) REFERENCES `hd_bank_ticket` (`ticket_type_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Các ràng buộc cho bảng `hd_bank_users`
--
ALTER TABLE `hd_bank_users`
  ADD CONSTRAINT `FK_user_group` FOREIGN KEY (`group_id`) REFERENCES `hd_bank_group` (`group_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Các ràng buộc cho bảng `hd_bank_users_modes`
--
ALTER TABLE `hd_bank_users_modes`
  ADD CONSTRAINT `fk_user_user_mode` FOREIGN KEY (`user_id`) REFERENCES `hd_bank_users` (`user_id`) ON DELETE CASCADE;

--
-- Các ràng buộc cho bảng `hd_bank_users_tag_modes`
--
ALTER TABLE `hd_bank_users_tag_modes`
  ADD CONSTRAINT `fk_user_user_tag_mode` FOREIGN KEY (`user_id`) REFERENCES `hd_bank_users` (`user_id`) ON DELETE CASCADE;

--
-- Các ràng buộc cho bảng `hd_bank_user_codes`
--
ALTER TABLE `hd_bank_user_codes`
  ADD CONSTRAINT `fK_user_user_codes` FOREIGN KEY (`user_id`) REFERENCES `hd_bank_users` (`user_id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
