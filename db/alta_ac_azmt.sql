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
-- Cơ sở dữ liệu: `alta_ac_azmt`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_devices`
--

CREATE TABLE `hdbank_ac_devices` (
  `device_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `device_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `device_password` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `device_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `device_mac` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `device_type_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `device_created` datetime DEFAULT NULL,
  `device_settings` longtext COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_devices`
--

INSERT INTO `hdbank_ac_devices` (`device_id`, `device_code`, `device_password`, `device_name`, `device_mac`, `device_type_id`, `device_created`, `device_settings`) VALUES
('037ae40e-0641-4d8f-83be-43f741c0666d', 'Phong_BA', '4297f44b13955235245b2497399d7a93', 'Phong BA', '222.222.222.222', 'e25ab0f6-ff05-476b-b165-4806c2f716ab', '2021-03-12 07:04:11', '{\"MQTT\":\"123123123\"}'),
('4511bb7a-9f91-441e-865c-79fcc9a6dee9', 'Phong LapTrinh', 'f5bb0c8de146c67b44babbf4e6584cc0', 'Phong LapTrinh', '123123', 'e25ab0f6-ff05-476b-b165-4806c2f716ab', '2021-03-12 03:33:10', 'null'),
('b343bfaa-2bb1-4bd5-8c5f-45f5a29bbdb1', 'CAFE_ROOM', '', 'Phong Cafe', '222.222.222.222', '5610c706-c113-43b4-85f0-5a9eca5bdb34', '2021-03-08 06:05:29', '{}');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_device_types`
--

CREATE TABLE `hdbank_ac_device_types` (
  `device_type_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `device_type_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `device_type_status` int(2) NOT NULL DEFAULT 1 COMMENT '0 = unactive; 1 = Active ',
  `device_type_created` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_device_types`
--

INSERT INTO `hdbank_ac_device_types` (`device_type_id`, `device_type_name`, `device_type_status`, `device_type_created`) VALUES
('5610c706-c113-43b4-85f0-5a9eca5bdb34', 'Magnetic card scanner', 1, '2020-12-22 15:25:41'),
('e25ab0f6-ff05-476b-b165-4806c2f716ab', 'Fingerprint scanner', 1, '2020-12-22 15:24:28'),
('fc3c0016-cab8-4555-ba2c-98e13d26ad7b', 'Camera', 1, '2020-12-22 15:24:28');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_mode`
--

CREATE TABLE `hdbank_ac_mode` (
  `mode_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_status` smallint(6) NOT NULL DEFAULT 1,
  `mode_created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_mode`
--

INSERT INTO `hdbank_ac_mode` (`mode_id`, `mode_name`, `mode_status`, `mode_created_at`) VALUES
('Card_ID', 'Card ID', 1, '2020-12-22 09:23:36'),
('Face_ID', 'Face ID', 1, '2020-12-22 09:23:36'),
('Finger_Print', 'Finger Print', 1, '2020-12-22 09:23:36');

--
-- Bẫy `hdbank_ac_mode`
--
DELIMITER $$
CREATE TRIGGER `tg_delete_ac_mode` AFTER DELETE ON `hdbank_ac_mode` FOR EACH ROW if( old.mode_id = 'Card_ID' OR old.mode_id = 'Face_ID' OR old.mode_id = 'Finger_Print')
	THEN
    signal sqlstate '42000' set message_text = 		"mode defaul not delete";
END IF
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_register_detects`
--

CREATE TABLE `hdbank_ac_register_detects` (
  `rg_detect_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `rg_detect_user_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tag_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `mode_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `rg_detect_created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `rg_detect_updated_at` datetime DEFAULT NULL,
  `rg_detect_key` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `rg_detect_extension` longtext COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_register_detects`
--

INSERT INTO `hdbank_ac_register_detects` (`rg_detect_id`, `rg_detect_user_id`, `ticket_type_id`, `tag_code`, `mode_id`, `rg_detect_created_at`, `rg_detect_updated_at`, `rg_detect_key`, `rg_detect_extension`) VALUES
('32145156123624634567357357', 'a8975049-9419-4b7a-9b8e-e72cad913c3d', '14341913-d0e6-43df-aaf9-ec5e3188e898', 'default_tag', 'Card_ID', '2021-03-29 11:55:43', NULL, '123123123', NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_register_detect_details`
--

CREATE TABLE `hdbank_ac_register_detect_details` (
  `rg_detect_detail_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `rg_detect_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `rg_detect_detail_time_begin` time NOT NULL,
  `rg_detect_detail_time_end` time NOT NULL,
  `rg_detect_detail_created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `rg_detect_detail_date_end` datetime NOT NULL,
  `rg_detect_detail_date_begin` datetime NOT NULL,
  `rg_detect_detail_repeat` int(11) NOT NULL,
  `rg_detect_detail_repeat_value` longtext COLLATE utf8_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Bẫy `hdbank_ac_register_detect_details`
--
DELIMITER $$
CREATE TRIGGER `hdbank_delete_parent` AFTER DELETE ON `hdbank_ac_register_detect_details` FOR EACH ROW BEGIN
	DECLARE dem INT;
	SELECT COUNT(*) INTO dem FROM hdbank_ac_register_detect_details as dt 
    WHERE  rg_detect_id = old.rg_detect_id;
   
   	IF dem = 0 THEN
    	DELETE FROM  hdbank_ac_register_detects  WHERE rg_detect_id = old.rg_detect_id;
    END IF;
	
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_tags`
--

CREATE TABLE `hdbank_ac_tags` (
  `tag_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `tag_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `tag_description` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `tag_time_start` time DEFAULT NULL,
  `tag_time_stop` time DEFAULT NULL,
  `tag_date_start` date DEFAULT NULL,
  `tag_date_stop` date DEFAULT NULL,
  `tag_repeat` int(11) NOT NULL,
  `tag_value` text COLLATE utf8_unicode_ci DEFAULT NULL,
  `tag_type` int(11) NOT NULL DEFAULT 0 COMMENT '1: CRM -- 0: AccessControll',
  `tag_status` int(11) NOT NULL DEFAULT 1 COMMENT '1: Active - 0: Unactive',
  `tag_created` datetime NOT NULL,
  `tag_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `repository_id` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_tags`
--

INSERT INTO `hdbank_ac_tags` (`tag_id`, `ticket_type_id`, `tag_name`, `tag_description`, `tag_time_start`, `tag_time_stop`, `tag_date_start`, `tag_date_stop`, `tag_repeat`, `tag_value`, `tag_type`, `tag_status`, `tag_created`, `tag_code`, `repository_id`) VALUES
('8188afe1-159a-4c6c-bd06-0c40370a46d5', '14341913-d0e6-43df-aaf9-ec5e3188e898', 'backend_Test', NULL, NULL, NULL, '2021-03-28', '2021-04-23', 1, '[]', 1, 0, '2021-03-29 04:47:26', 'default_tag', 'b50e47b4-4ca5-400e-b776-63d96c2a79f7');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_ticket_types`
--

CREATE TABLE `hdbank_ac_ticket_types` (
  `ticket_type_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_description` varchar(1000) COLLATE utf8_unicode_ci DEFAULT NULL,
  `ticket_type_status` int(11) NOT NULL DEFAULT 1 COMMENT '1: Active - 0: Unactive',
  `ticket_type_created` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_ticket_types`
--

INSERT INTO `hdbank_ac_ticket_types` (`ticket_type_id`, `ticket_type_name`, `ticket_type_description`, `ticket_type_status`, `ticket_type_created`) VALUES
('14341913-d0e6-43df-aaf9-ec5e3188e898', 'ticket_new', 'ticket_new', 1, '2021-03-29 04:48:03'),
('21e9fde0-f980-44e8-8840-8e4b845c9c45', 'test hoa', 'test hoa', 1, '2021-03-17 10:44:29');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `hdbank_ac_ticket_type_devices`
--

CREATE TABLE `hdbank_ac_ticket_type_devices` (
  `ticket_type_device_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `device_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `ticket_type_device_created_at` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `hdbank_ac_ticket_type_devices`
--

INSERT INTO `hdbank_ac_ticket_type_devices` (`ticket_type_device_id`, `ticket_type_id`, `device_id`, `ticket_type_device_created_at`) VALUES
('17ff0799-c745-424c-a5d6-75770b3c723d', '14341913-d0e6-43df-aaf9-ec5e3188e898', '037ae40e-0641-4d8f-83be-43f741c0666d', '2021-03-29 04:48:03'),
('cbfd7bb4-5989-4d9f-bd9b-49271d303c33', '21e9fde0-f980-44e8-8840-8e4b845c9c45', '4511bb7a-9f91-441e-865c-79fcc9a6dee9', '2021-03-17 10:44:29'),
('cf441485-c7aa-438d-8986-afbbab18610a', '14341913-d0e6-43df-aaf9-ec5e3188e898', '4511bb7a-9f91-441e-865c-79fcc9a6dee9', '2021-03-29 04:48:03'),
('e813ebdf-2622-45eb-a247-5a2f1a8edfb1', '14341913-d0e6-43df-aaf9-ec5e3188e898', 'b343bfaa-2bb1-4bd5-8c5f-45f5a29bbdb1', '2021-03-29 04:48:03');

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `hdbank_ac_devices`
--
ALTER TABLE `hdbank_ac_devices`
  ADD PRIMARY KEY (`device_id`),
  ADD KEY `fk_type_device` (`device_type_id`);

--
-- Chỉ mục cho bảng `hdbank_ac_device_types`
--
ALTER TABLE `hdbank_ac_device_types`
  ADD PRIMARY KEY (`device_type_id`);

--
-- Chỉ mục cho bảng `hdbank_ac_mode`
--
ALTER TABLE `hdbank_ac_mode`
  ADD PRIMARY KEY (`mode_id`);

--
-- Chỉ mục cho bảng `hdbank_ac_register_detects`
--
ALTER TABLE `hdbank_ac_register_detects`
  ADD PRIMARY KEY (`rg_detect_id`);

--
-- Chỉ mục cho bảng `hdbank_ac_register_detect_details`
--
ALTER TABLE `hdbank_ac_register_detect_details`
  ADD PRIMARY KEY (`rg_detect_detail_id`),
  ADD KEY `fk_register_register_detail` (`rg_detect_id`);

--
-- Chỉ mục cho bảng `hdbank_ac_tags`
--
ALTER TABLE `hdbank_ac_tags`
  ADD PRIMARY KEY (`tag_id`),
  ADD UNIQUE KEY `tag_code_unique` (`tag_code`);

--
-- Chỉ mục cho bảng `hdbank_ac_ticket_types`
--
ALTER TABLE `hdbank_ac_ticket_types`
  ADD PRIMARY KEY (`ticket_type_id`);

--
-- Chỉ mục cho bảng `hdbank_ac_ticket_type_devices`
--
ALTER TABLE `hdbank_ac_ticket_type_devices`
  ADD PRIMARY KEY (`ticket_type_device_id`),
  ADD KEY `fk_device_ticket_device` (`device_id`),
  ADD KEY `fk_tag_ticket_device` (`ticket_type_id`);

--
-- Các ràng buộc cho các bảng đã đổ
--

--
-- Các ràng buộc cho bảng `hdbank_ac_devices`
--
ALTER TABLE `hdbank_ac_devices`
  ADD CONSTRAINT `fk_type_device` FOREIGN KEY (`device_type_id`) REFERENCES `hdbank_ac_device_types` (`device_type_id`);

--
-- Các ràng buộc cho bảng `hdbank_ac_register_detect_details`
--
ALTER TABLE `hdbank_ac_register_detect_details`
  ADD CONSTRAINT `fk_register_register_detail` FOREIGN KEY (`rg_detect_id`) REFERENCES `hdbank_ac_register_detects` (`rg_detect_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Các ràng buộc cho bảng `hdbank_ac_ticket_type_devices`
--
ALTER TABLE `hdbank_ac_ticket_type_devices`
  ADD CONSTRAINT `fk_device_ticket_device` FOREIGN KEY (`device_id`) REFERENCES `hdbank_ac_devices` (`device_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `fk_tag_ticket_device` FOREIGN KEY (`ticket_type_id`) REFERENCES `hdbank_ac_ticket_types` (`ticket_type_id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
