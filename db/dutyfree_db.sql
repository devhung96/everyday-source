-- phpMyAdmin SQL Dump
-- version 5.0.1
-- https://www.phpmyadmin.net/
--
-- Máy chủ: 192.168.11.42
-- Thời gian đã tạo: Th3 31, 2021 lúc 04:05 PM
-- Phiên bản máy phục vụ: 10.4.11-MariaDB-1:10.4.11+maria~bionic
-- Phiên bản PHP: 7.2.19-0ubuntu0.18.04.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Cơ sở dữ liệu: `dutyfree_db`
--

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_airport`
--

CREATE TABLE `wh_airport` (
  `airport_code` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `airport_name` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `country_code` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `airport_altitude` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL,
  `airport_latitude` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '0',
  `airport_longtitude` varchar(128) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_airport`
--

INSERT INTO `wh_airport` (`airport_code`, `airport_name`, `country_code`, `airport_altitude`, `airport_latitude`, `airport_longtitude`) VALUES
('ALA ', 'ALMATY                                  ', 'UA', '2233', '43.3552777777778', '77.0447222222222'),
('AMD ', 'AHMEDABAD                               ', 'V ', '180', '23.0711', '-72.6264'),
('AMS ', 'AMSTERDAM,SCHIPHOL                      ', 'EH', '-11', '52.3080555555556', '4.76416666666667'),
('AOJ ', 'AOMORI                                  ', 'R ', '652', '40.7333333333333', '140.688611111111'),
('ATH ', 'ATHENS, VENIZELOS INTL                  ', 'LG', '94', '37.9367', '-23.9444'),
('AZI ', 'ABU DHABI AL BATEEN                     ', 'OM', '0', '24.4283', '-54.4581'),
('BAV ', 'BAOTOU                                  ', 'Z ', '0', '40.0667', '-109.983'),
('BKK ', 'BANGKOK INTL                            ', 'VT', '5', '13.9144', '-100.606'),
('BLR ', 'BANGALORE                               ', 'V ', '2914', '12.9519', '-77.6644'),
('BMV ', 'BAN ME THUOT                            ', 'VV', '0', '12.0667', '-108.1'),
('BOM ', 'MUMBAI,CHHATRAPATI SHIVAJI INTL         ', 'V ', '27', '19.0914', '-72.8658'),
('CGK ', 'JAKARTA,SOEKARNO  HATTA INTL            ', 'W ', '34', '-6.12361111111111', '-106.661111111111'),
('CGO ', 'ZHENGZHOU,XINZHENG                      ', 'Z ', '495', '34.5183', '-113.842'),
('CGQ ', 'CHANGCHUN,DAFANGSHEN                    ', 'Z ', '758', '43.9983', '-125.688'),
('CJJ ', 'CHEONGJU                                ', 'RK', '166', '36.7206', '-127.494'),
('CKG ', 'CHONGQING,JIANGBEI                      ', 'Z ', '1365', '29.7167', '-106.64'),
('CNX ', 'CHIANG MAI INTL                         ', 'VT', '1036', '18.7714', '-98.9628'),
('CSX ', 'CHANGSHA,HUANGHUA                       ', 'Z ', '217', '28.1867', '-113.222'),
('CTS ', 'SAPPORO,NEW CHITOSE                     ', 'R ', '70', '42.7753', '-141.693'),
('CTU ', 'CHENGDU,SHUANGLIU                       ', 'Z ', '1624', '30.58', '-103.948'),
('CXR ', 'CAM RANH                                ', 'VV', '0', '11.9931', '-109.225'),
('CXT ', 'CHARTERS TOWERS                         ', 'Y ', '0', '-20.0430555555556', '146.273055555556'),
('CZX ', 'CHANGZHOU                               ', 'Z ', '0', '31.7', '-119.967'),
('DAD ', 'DANANG INTL                             ', 'VV', '33', '16.0439', '-108.2'),
('DEL ', 'DELHI,INDIRA GANDHI INTL                ', 'V ', '744', '28.5686', '-77.1119'),
('DLC ', 'DALIAN,ZHOUSHUIZI                       ', 'Z ', '108', '38.9633', '-121.537'),
('DLI ', 'DALAT                                   ', 'VV', '0', '11.75', '-108.367'),
('DMK ', 'DON MUANG                               ', 'VT', '9', '13.9125', '100.606666666667'),
('DOH ', 'DOHA INTL                               ', 'OT', '35', '25.2608333333333', '51.565'),
('DPS ', 'DEN PASAR,BALI INTL                     ', 'W ', '14', '-8.7475', '-115.169'),
('DSN ', 'DONGSHENG                               ', 'Z ', '0', '39.85', '-110.033'),
('DXB ', 'DUBAI INTL                              ', 'OM', '34', '25.2527777777778', '55.3644444444444'),
('DYG ', 'DAYONG                                  ', 'Z ', '0', '29.1333', '-110.483'),
('FKS ', 'FUKUSHIMA                               ', 'R ', '1220', '37.2275', '-140.428'),
('FOC ', 'FUZHOU,CHANGLE                          ', 'Z ', '46', '25.9333', '-119.662'),
('FUK ', 'FUKUOKA                                 ', 'R ', '30', '33.5844444444444', '130.451666666667'),
('GAY ', 'GAYA                                    ', 'V ', '362', '24.7475', '-84.9447'),
('GMP ', 'SEOUL, GIMPO INTL                       ', 'RK', '58', '37.5580555555556', '-126.790555555556'),
('HA  ', 'HAN                                     ', 'VV', '39', '21.2216666666667', '105.805555555556'),
('HAK ', 'HAIKOU,MEILAN                           ', 'Z ', '75', '19.9333', '-110.457'),
('HAM ', 'HAMBURG                                 ', 'ED', '53', '53.6302777777778', '9.98805555555555'),
('HAN ', 'HANOI,NOIBAI INTL                       ', 'VV', '39', '21.2217', '-105.806'),
('HET ', 'HOHHOT,BAITA                            ', 'Z ', '3556', '40.8517', '-111.825'),
('HFE ', 'HEFEI,LUOGANG                           ', 'Z ', '95', '31.7833', '-117.3'),
('HGH ', 'HANGZHOU,XIAOSHAN                       ', 'Z ', '23', '30.2283', '-120.432'),
('HHA ', 'HUANGHUA                                ', 'Z ', '0', '28.1867', '-113.222'),
('HIA ', 'HUAI\'AN LIANSHUI                        ', 'Z ', '26', '33.7875', '-119.135'),
('HKG ', 'HONG KONG INTL                          ', 'VH', '19', '22.3089', '-113.915'),
('HKT ', 'PHUKET INTL                             ', 'VT', '82', '8.1125', '-98.3092'),
('HND ', 'TOKYO (HANEDA) INTL                     ', 'R ', '21', '35.5533333333333', '139.781111111111'),
('HPH ', 'HAIPHONG                                ', 'VV', '0', '20.8', '-106.65'),
('HRB ', 'HARBIN,TAIPING                          ', 'Z ', '456', '45.6167', '-126.25'),
('HUI ', 'HUE                                     ', 'VV', '0', '16.4022', '-107.702'),
('IBR ', 'OMITAMA IBARAKI                         ', 'R ', '110', '36.1778', '-140.408'),
('ICN ', 'SEOUL  INCHEON,INCHEON INTL             ', 'RK', '23', '37.4625', '-126.439'),
('INC ', 'YINCHUAN                                ', 'Z ', '0', '38.3217', '-106.392'),
('JJN ', 'JINJIANG                                ', 'Z ', '0', '24.9', '-118.583'),
('KHH ', 'KAOHSIUNG INTL                          ', 'RC', '31', '22.5769', '-120.35'),
('KHN ', 'NANCHANG                                ', 'Z ', '0', '28.865', '-115.9'),
('KIJ ', 'NIIGATA                                 ', 'R ', '5', '37.9558', '-139.112'),
('KIX ', 'OSAKA,KANSAI INTL                       ', 'R ', '15', '34.4342', '-135.233'),
('KMG ', 'KUNMING,WUJIABA                         ', 'Z ', '6217', '25', '-102.75'),
('KMQ ', 'KOMATSU                                 ', 'R ', '18', '36.3936', '-136.408'),
('KUL ', 'KUALA LUMPUR INTL SEPANG                ', 'WM', '70', '2.74333', '-101.698'),
('KUN ', 'KAUNAS INTL                             ', 'EY', '256', '54.9638888888889', '24.0847222222222'),
('KWE ', 'GUIYANG                                 ', 'Z ', '0', '26.5367', '-106.803'),
('LHW ', 'LANZHOU                                 ', 'Z ', '45', '31.8881', '81.5614'),
('LYI ', 'LINYI SHUBULING                         ', 'Z ', '204', '35.0894', '-118.692'),
('MFM ', 'MACAU INTL                              ', 'VM', '20', '22.1494', '-113.591'),
('MIA ', 'MIAMI INTL,FL                           ', 'K ', '8', '25.7953', '80.29'),
('MNL ', 'MANILA,NINOY AQUINO INTL                ', 'RP', '0', '14.5097222222222', '121.013611111111'),
('MST ', 'MAASTRICHT  AACHEN                      ', 'EH', '375', '50.9158333333333', '5.77694444444444'),
('MWX ', 'MUAN INTERNATIONAL                      ', 'RK', '39', '34.9933', '-126.388'),
('NGB ', 'NINGBO,LISHE                            ', 'Z ', '13', '29.8233', '-121.463'),
('NGO ', 'NAGOYA                                  ', 'R ', '46', '34.8583', '-136.805'),
('NKG ', 'NANJING,LUKOU                           ', 'Z ', '49', '31.74', '-118.86'),
('NNG ', 'NANNING,WUXU                            ', 'Z ', '420', '22.6167', '-108.183'),
('NRT ', 'TOKYO INTL  NARITA                      ', 'R ', '135', '35.7653', '-140.386'),
('NTG ', 'NANTONG                                 ', 'Z ', '0', '32.0667', '-120.967'),
('PEN ', 'PENANG INTL                             ', 'WM', '11', '5.29722', '-100.277'),
('PQC ', 'PHUQUOC                                 ', 'VV', '0', '10.25', '-104'),
('PUS ', 'BUSAN,GIMHAE INTL                       ', 'RK', '13', '35.1806', '-128.938'),
('PVG ', 'SHANGHAI,PUDONG                         ', 'Z ', '10', '31.1417', '-121.79'),
('PXU ', 'PLEIKU                                  ', 'VV', '0', '14', '-108'),
('REP ', 'SIEM REAP, CAMBODIA,SIEM REAP           ', 'VD', '60', '13.4106', '-103.813'),
('RGN ', 'YANGON INTL                             ', 'VY', '109', '16.9072', '-96.1331'),
('RMQ ', 'TAICHUNG  INTL                          ', 'RC', '663', '24.2547', '-120.601'),
('ROR ', 'BABELTHUAP I,BABELTHUAP  KOROR          ', 'PW', '176', '7.36722', '-134.544'),
('SDJ ', 'SENDAI                                  ', 'R ', '6', '38.1397', '-140.917'),
('SGM ', 'SAN IGNACIO                             ', 'MM', '0', '27.2833333333333', '-112.933333333333'),
('SGN ', 'HOCHIMINH,TANSONNHAT                    ', 'VV', '33', '10.8206', '-106.661'),
('SHE ', 'SHENYANG,TAOXIAN                        ', 'Z ', '197', '41.6383', '-123.485'),
('SIN ', 'SINGAPORE,CHANGI                        ', 'WS', '22', '1.35917', '-103.989'),
('SJW ', 'SHIJIAZHUANG,ZHENGDING                  ', 'Z ', '233', '38.2783', '-114.698'),
('SWA ', 'SHANTOU                                 ', 'Z ', '0', '23.4283', '-116.763'),
('TAE ', 'DAEGU                                   ', 'RK', '116', '35.8939', '-128.659'),
('TAO ', 'QINGDAO,LIUTING                         ', 'Z ', '33', '36.265', '-120.375'),
('TBB ', 'TUY HOA                                 ', 'VV', '0', '13.05', '-109.033'),
('THD ', 'SAO VANG                                ', 'VV', '72', '19.8869', '-105.463'),
('TLS ', 'TOULOUSE,BLAGNAC                        ', 'LF', '499', '43.635', '-1.36778'),
('TNA ', 'JINAN,YAOQIANG                          ', 'Z ', '75', '36.855', '-117.217'),
('TNN ', 'TAINAN AERO                             ', 'RC', '63', '22.9492', '-120.211'),
('TPE ', 'TAIPEI,TAOYUAN INTL                     ', 'RC', '107', '25.0803', '-121.232'),
('TSN ', 'TIANJIN,BINHAI                          ', 'Z ', '10', '39.125', '-117.347'),
('TYN ', 'TAIYUAN,WUSU                            ', 'Z ', '2575', '37.7483', '-112.63'),
('UIH ', 'QUINHON                                 ', 'VV', '0', '13.7667', '-109.233'),
('UYN ', 'YULIN                                   ', 'Z ', '0', '38.0333', '-109.483'),
('VCA ', 'CAN THO                                 ', 'VV', '0', '10.05', '-105.767'),
('VCL ', 'CHU LAI INTL                            ', 'VV', '25', '15.4086', '-108.704'),
('VDH ', 'DONG HOI                                ', 'VV', '60', '17.515', '-106.59'),
('VDO ', 'VAN DON INTL                            ', 'VV', '0', '21.1178', '-107.414'),
('VII ', 'VINH CITY                               ', 'VV', '0', '18.7333', '-105.067'),
('WAW ', 'WARSAW,OKECIE                           ', 'EP', '0', '52.1655555555556', '20.9669444444444'),
('WNZ ', 'WENZHOU                                 ', 'Z ', '0', '28', '-121.067'),
('WUH ', 'WUHAN,TIANHE                            ', 'Z ', '112', '30.785', '-114.21'),
('WUX ', 'WUXI                                    ', 'Z ', '0', '31.5833', '-120.317'),
('XFW ', 'HAMBURG,FINKENWERDER                    ', 'ED', '16', '53.5353', '-9.83528'),
('XIY ', 'XIAN,XIANYANG                           ', 'Z ', '1572', '34.445', '-108.75'),
('XUZ ', 'XUZHOU                                  ', 'Z ', '0', '34.2667', '-117.183'),
('YGJ ', 'YONAGO,MIHO AERO                        ', 'R ', '12', '35.4933333333333', '133.239166666667'),
('YIH ', 'YICHANG                                 ', 'Z ', '0', '30.7', '-111.183'),
('YIW ', 'YIWU                                    ', 'Z ', '0', '29.2667', '-120.05'),
('YNT ', 'YANTAI,LAISHAN                          ', 'Z ', '59', '37.4017', '-121.372'),
('YNY ', 'YANGYANG INTL                           ', 'RK', '0', '38.0586', '-128.663'),
('YTY ', 'YANGZHOU TAIZHOU                        ', 'Z ', '16', '32.5616666666667', '119.715'),
('ZAG ', 'ZAGREB,PLESO                            ', 'LD', '353', '45.7427777777778', '16.0686111111111'),
('ZGC ', 'LANZHOU ZHONGCHUAN                      ', 'Z ', '0', '36.5167', '-103.622'),
('ZYI ', 'ZUNYI                                   ', 'Z ', '0', '27.0667', '-106.083');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_cityPair`
--

CREATE TABLE `wh_cityPair` (
  `citypair_id` int(11) NOT NULL,
  `citypair_route` varchar(255) NOT NULL,
  `citypair_status` int(11) NOT NULL DEFAULT 1,
  `citypair_date_start` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `citypair_schedule` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`citypair_schedule`)),
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `order` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Đang đổ dữ liệu cho bảng `wh_cityPair`
--

INSERT INTO `wh_cityPair` (`citypair_id`, `citypair_route`, `citypair_status`, `citypair_date_start`, `citypair_schedule`, `created_at`, `updated_at`, `order`) VALUES
(1, 'SGN-BKK', 1, '2019-01-29 17:00:00', '[{\"departure\":\"SGN\",\"arrival\":\"BKK\",\"route\":null,\"typeSchedule\":0,\"flightNumber\":\"VJ801\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"BKK\",\"arrival\":\"SGN\",\"route\":null,\"typeSchedule\":1,\"flightNumber\":\"VJ802\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SGN\",\"arrival\":\"BKK\",\"route\":null,\"typeSchedule\":0,\"flightNumber\":\"VJ803\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"BKK\",\"arrival\":\"SGN\",\"route\":null,\"typeSchedule\":1,\"flightNumber\":\"VJ804\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SGN\",\"arrival\":\"BKK\",\"route\":null,\"typeSchedule\":0,\"flightNumber\":\"VJ805\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"BKK\",\"arrival\":\"SGN\",\"route\":null,\"typeSchedule\":1,\"flightNumber\":\"VJ806\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:22', '2020-08-18 09:10:22', 2),
(2, 'SGN-HKT', 1, '2019-01-29 17:00:00', '[{\"Departure\":\"HKT\",\"Arrival\":\"SGN\",\"Route\":null,\"TypeSchedule\":1,\"FlightNumber\":\"VJ808\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"SGN\",\"Arrival\":\"HKT\",\"Route\":null,\"TypeSchedule\":0,\"FlightNumber\":\"VJ809\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(3, 'SGN-SIN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"SIN\",\"route\":\"SGN-SIN\",\"typeSchedule\":0,\"flightNumber\":\"VJ811\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"SIN\",\"arrival\":\"SGN\",\"route\":\"SIN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ812\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"SGN\",\"arrival\":\"SIN\",\"route\":\"SGN-SIN\",\"typeSchedule\":0,\"flightNumber\":\"VJ813\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SIN\",\"arrival\":\"SGN\",\"route\":\"SIN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ814\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(4, 'SGN-NRT', 1, '2020-08-17 17:00:00', '[{\"departure\":\"SGN\",\"arrival\":\"NRT\",\"route\":null,\"typeSchedule\":0,\"flightNumber\":\"VJ822\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(5, 'SGN-KUL', 1, '2020-08-17 17:00:00', '[{\"departure\":\"SGN\",\"arrival\":\"KUL\",\"route\":null,\"typeSchedule\":0,\"flightNumber\":\"VJ825\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"KUL\",\"arrival\":\"SGN\",\"route\":null,\"typeSchedule\":1,\"flightNumber\":\"VJ826\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(6, 'SGN-KIX', 1, '2020-10-23 07:30:51', '[{\"departure\":\"KIX\",\"arrival\":\"SGN\",\"route\":null,\"typeSchedule\":1,\"flightNumber\":\"VJ829\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SGN\",\"arrival\":\"KIX\",\"route\":null,\"typeSchedule\":0,\"flightNumber\":\"VJ828\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(7, 'CXR-ICN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"CXR\",\"arrival\":\"ICN\",\"route\":\"CXR-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ836\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\"]},{\"departure\":\"ICN\",\"arrival\":\"CXR\",\"route\":\"ICN-CXR\",\"typeSchedule\":1,\"flightNumber\":\"VJ837\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\"]},{\"departure\":\"CXR\",\"arrival\":\"ICN\",\"route\":\"CXR-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ838\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"CXR\",\"route\":\"ICN-CXR\",\"typeSchedule\":1,\"flightNumber\":\"VJ839\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(8, 'SGN-TPE', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"TPE\",\"route\":\"SGN-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ840\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"TPE\",\"arrival\":\"SGN\",\"route\":\"TPE-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ841\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"SGN\",\"arrival\":\"TPE\",\"route\":\"SGN-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ842\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"TPE\",\"arrival\":\"SGN\",\"route\":\"TPE-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ843\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SGN\",\"arrival\":\"TPE\",\"route\":\"SGN-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ844\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\"]},{\"departure\":\"TPE\",\"arrival\":\"SGN\",\"route\":\"TPE-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ845\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(9, 'SGN-RMQ', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"RMQ\",\"route\":\"SGN-RMQ\",\"typeSchedule\":0,\"flightNumber\":\"VJ850\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\"]},{\"departure\":\"RMQ\",\"arrival\":\"SGN\",\"route\":\"RMQ-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ851\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\"]},{\"departure\":\"SGN\",\"arrival\":\"RMQ\",\"route\":\"SGN-RMQ\",\"typeSchedule\":0,\"flightNumber\":\"VJ852\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"RMQ\",\"arrival\":\"SGN\",\"route\":\"RMQ-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ853\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(10, 'SGN-TNN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"TNN\",\"route\":\"SGN-TNN\",\"typeSchedule\":0,\"flightNumber\":\"VJ858\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Thu\",\"Fri\",\"Sun\"]},{\"departure\":\"TNN\",\"arrival\":\"SGN\",\"route\":\"TNN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ859\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Thu\",\"Fri\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(11, 'SGN-ICN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"ICN\",\"route\":\"SGN-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ860\",\"status\":1,\"flightTime\":[\"Mon\",\"Wed\"]},{\"departure\":\"ICN\",\"arrival\":\"SGN\",\"route\":\"ICN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ861\",\"status\":1,\"flightTime\":[\"Mon\",\"Wed\"]},{\"departure\":\"SGN\",\"arrival\":\"ICN\",\"route\":\"SGN-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ862\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"SGN\",\"route\":\"ICN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ863\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SGN\",\"arrival\":\"ICN\",\"route\":\"SGN-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ864\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"SGN\",\"route\":\"ICN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ865\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(12, 'VCA-TPE', 1, '2020-08-18 09:10:23', '[{\"departure\":\"VCA\",\"arrival\":\"TPE\",\"route\":\"VCA-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ866\",\"status\":1,\"flightTime\":[\"Mon\"]},{\"departure\":\"TPE\",\"arrival\":\"VCA\",\"route\":\"TPE-VCA\",\"typeSchedule\":1,\"flightNumber\":\"VJ867\",\"status\":1,\"flightTime\":[\"Mon\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(13, 'DAD-TAE', 1, '2020-08-18 09:10:23', '[{\"departure\":\"DAD\",\"arrival\":\"TAE\",\"route\":\"DAD-TAE\",\"typeSchedule\":0,\"flightNumber\":\"VJ870\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"TAE\",\"arrival\":\"DAD\",\"route\":\"TAE-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ871\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(14, 'DAD-ICN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"DAD\",\"arrival\":\"ICN\",\"route\":\"DAD-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ874\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"DAD\",\"route\":\"ICN-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ875\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"DAD\",\"arrival\":\"ICN\",\"route\":\"DAD-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ878\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"DAD\",\"route\":\"ICN-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ879\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"DAD\",\"arrival\":\"ICN\",\"route\":\"DAD-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ880\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Sat\"]},{\"departure\":\"ICN\",\"arrival\":\"DAD\",\"route\":\"ICN-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ881\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Sat\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(15, 'SGN-KHH', 1, '2020-08-18 09:10:23', '[{\"departure\":\"KHH\",\"arrival\":\"SGN\",\"route\":\"KHH-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ885\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SGN\",\"arrival\":\"KHH\",\"route\":\"SGN-KHH\",\"typeSchedule\":0,\"flightNumber\":\"VJ886\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"KHH\",\"arrival\":\"SGN\",\"route\":\"KHH-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ889\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\"]},{\"departure\":\"SGN\",\"arrival\":\"KHH\",\"route\":\"SGN-KHH\",\"typeSchedule\":0,\"flightNumber\":\"VJ890\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(16, 'SGN-DPS', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"DPS\",\"route\":\"SGN-DPS\",\"typeSchedule\":0,\"flightNumber\":\"VJ893\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"DPS\",\"arrival\":\"SGN\",\"route\":\"DPS-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ894\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(17, 'SGN-DEL', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"DEL\",\"route\":\"SGN-DEL\",\"typeSchedule\":0,\"flightNumber\":\"VJ895\",\"status\":1,\"flightTime\":[\"Mon\",\"Wed\",\"Fri\",\"Sun\"]},{\"departure\":\"DEL\",\"arrival\":\"SGN\",\"route\":\"DEL-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ896\",\"status\":1,\"flightTime\":[\"Mon\",\"Wed\",\"Fri\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(18, 'HAN-BKK', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"BKK\",\"route\":\"HAN-BKK\",\"typeSchedule\":0,\"flightNumber\":\"VJ901\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"BKK\",\"arrival\":\"HAN\",\"route\":\"BKK-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ902\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(19, 'DAD-TPE', 1, '2020-08-18 09:10:23', '[{\"departure\":\"DAD\",\"arrival\":\"TPE\",\"route\":\"DAD-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ908\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"TPE\",\"arrival\":\"DAD\",\"route\":\"TPE-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ909\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(20, 'HAN-REP', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"REP\",\"route\":\"HAN-REP\",\"typeSchedule\":0,\"flightNumber\":\"VJ913\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"REP\",\"arrival\":\"HAN\",\"route\":\"REP-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ914\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(21, 'HAN-SIN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"SIN\",\"route\":\"HAN-SIN\",\"typeSchedule\":0,\"flightNumber\":\"VJ915\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SIN\",\"arrival\":\"HAN\",\"route\":\"SIN-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ916\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(22, 'HAN-RGN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"RGN\",\"route\":\"HAN-RGN\",\"typeSchedule\":0,\"flightNumber\":\"VJ917\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"RGN\",\"arrival\":\"HAN\",\"route\":\"RGN-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ918\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(23, 'HPH-ICN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"ICN\",\"arrival\":\"HPH\",\"route\":\"ICN-HPH\",\"typeSchedule\":1,\"flightNumber\":\"VJ925\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"HPH\",\"arrival\":\"ICN\",\"route\":\"HPH-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ926\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(24, 'HAN-NRT', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"NRT\",\"route\":\"HAN-NRT\",\"typeSchedule\":0,\"flightNumber\":\"VJ932\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"NRT\",\"arrival\":\"HAN\",\"route\":\"NRT-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ933\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(25, 'DAD-HND', 1, '2020-08-18 09:10:23', '[{\"departure\":\"DAD\",\"arrival\":\"HND\",\"route\":\"DAD-HND\",\"typeSchedule\":0,\"flightNumber\":\"VJ936\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"HND\",\"arrival\":\"DAD\",\"route\":\"HND-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ937\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(26, 'HAN-KIX', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"KIX\",\"route\":\"HAN-KIX\",\"typeSchedule\":0,\"flightNumber\":\"VJ938\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"KIX\",\"arrival\":\"HAN\",\"route\":\"KIX-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ939\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(27, 'HAN-TPE', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"TPE\",\"route\":\"HAN-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ940\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"TPE\",\"arrival\":\"HAN\",\"route\":\"TPE-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ941\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"HAN\",\"arrival\":\"TPE\",\"route\":\"HAN-TPE\",\"typeSchedule\":0,\"flightNumber\":\"VJ942\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Sun\"]},{\"departure\":\"TPE\",\"arrival\":\"HAN\",\"route\":\"TPE-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ943\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(28, 'HAN-KHH', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"KHH\",\"route\":\"HAN-KHH\",\"typeSchedule\":0,\"flightNumber\":\"VJ946\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"KHH\",\"arrival\":\"HAN\",\"route\":\"KHH-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ947\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(29, 'HAN-RMQ', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"RMQ\",\"route\":\"HAN-RMQ\",\"typeSchedule\":0,\"flightNumber\":\"VJ948\",\"status\":1,\"flightTime\":[\"Mon\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"RMQ\",\"arrival\":\"HAN\",\"route\":\"RMQ-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ949\",\"status\":1,\"flightTime\":[\"Mon\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(30, 'HAN-ICN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"HAN\",\"arrival\":\"ICN\",\"route\":\"HAN-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ960\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"HAN\",\"route\":\"ICN-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ961\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"HAN\",\"arrival\":\"ICN\",\"route\":\"HAN-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ962\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"HAN\",\"route\":\"ICN-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ963\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(31, 'DAD-SIN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SIN\",\"arrival\":\"DAD\",\"route\":\"SIN-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ970\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(32, 'PQC-ICN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"PQC\",\"arrival\":\"ICN\",\"route\":\"PQC-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ974\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Sat\"]},{\"departure\":\"ICN\",\"arrival\":\"PQC\",\"route\":\"ICN-PQC\",\"typeSchedule\":1,\"flightNumber\":\"VJ975\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Sat\"]},{\"departure\":\"PQC\",\"arrival\":\"ICN\",\"route\":\"PQC-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ978\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"ICN\",\"arrival\":\"PQC\",\"route\":\"ICN-PQC\",\"typeSchedule\":1,\"flightNumber\":\"VJ979\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(33, 'HAN-PUS', 1, '2020-08-18 09:10:23', '[{\"departure\":\"PUS\",\"arrival\":\"HAN\",\"route\":\"PUS-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ981\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"HAN\",\"arrival\":\"PUS\",\"route\":\"HAN-PUS\",\"typeSchedule\":0,\"flightNumber\":\"VJ982\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 1),
(34, 'CXR-PUS', 1, '2020-08-18 09:10:23', '[{\"departure\":\"CXR\",\"arrival\":\"PUS\",\"route\":\"CXR-PUS\",\"typeSchedule\":0,\"flightNumber\":\"VJ990\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Fri\",\"Sat\"]},{\"departure\":\"PUS\",\"arrival\":\"CXR\",\"route\":\"PUS-CXR\",\"typeSchedule\":1,\"flightNumber\":\"VJ991\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Fri\",\"Sat\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 0),
(35, 'SGN-CNX', 1, '2020-08-18 09:10:24', '[{\"departure\":\"SGN\",\"arrival\":\"CNX\",\"route\":\"SGN-CNX\",\"typeSchedule\":0,\"flightNumber\":\"VJ891\",\"status\":1,\"flightTime\":[\"Tue\",\"Fri\",\"Sun\"]},{\"departure\":\"CNX\",\"arrival\":\"SGN\",\"route\":\"CNX-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ892\",\"status\":1,\"flightTime\":[\"Tue\",\"Fri\",\"Sun\"]}]', '2020-08-18 09:10:24', '2020-08-18 09:10:24', 2),
(36, 'DLI-ICN', 1, '2020-08-18 09:10:24', '[{\"departure\":\"DLI\",\"arrival\":\"ICN\",\"route\":\"DLI-ICN\",\"typeSchedule\":0,\"flightNumber\":\"VJ944\",\"status\":1,\"flightTime\":[\"Tue\"]},{\"departure\":\"ICN\",\"arrival\":\"DLI\",\"route\":\"ICN-DLI\",\"typeSchedule\":1,\"flightNumber\":\"VJ945\",\"status\":1,\"flightTime\":[\"Tue\"]}]', '2020-08-18 09:10:24', '2020-08-18 09:10:24', 0),
(37, 'HAN-DEL', 1, '2020-08-18 09:10:24', '[{\"departure\":\"HAN\",\"arrival\":\"DEL\",\"route\":\"HAN-DEL\",\"typeSchedule\":0,\"flightNumber\":\"VJ971\",\"status\":1,\"flightTime\":[\"Tue\",\"Thu\",\"Sat\"]},{\"departure\":\"DEL\",\"arrival\":\"HAN\",\"route\":\"DEL-HAN\",\"typeSchedule\":1,\"flightNumber\":\"VJ972\",\"status\":1,\"flightTime\":[\"Tue\",\"Thu\",\"Sat\"]}]', '2020-08-18 09:10:24', '2020-08-18 09:10:24', 1),
(38, 'DAD-CJJ', 1, '2020-08-18 09:10:24', '[{\"departure\":\"DAD\",\"arrival\":\"CJJ\",\"route\":\"DAD-CJJ\",\"typeSchedule\":0,\"flightNumber\":\"VJ8764\",\"status\":1,\"flightTime\":[\"Tue\"]},{\"departure\":\"CJJ\",\"arrival\":\"DAD\",\"route\":\"CJJ-DAD\",\"typeSchedule\":1,\"flightNumber\":\"VJ8765\",\"status\":1,\"flightTime\":[\"Wed\"]}]', '2020-08-18 09:10:24', '2020-08-18 09:10:24', 0),
(39, 'HPH-BKK', 1, '2020-08-18 09:10:25', '[{\"departure\":\"HPH\",\"arrival\":\"BKK\",\"route\":\"HPH-BKK\",\"typeSchedule\":0,\"flightNumber\":\"VJ905\",\"status\":1,\"flightTime\":[\"Wed\",\"Fri\",\"Sun\"]},{\"departure\":\"BKK\",\"arrival\":\"HPH\",\"route\":\"BKK-HPH\",\"typeSchedule\":1,\"flightNumber\":\"VJ906\",\"status\":1,\"flightTime\":[\"Wed\",\"Fri\",\"Sun\"]}]', '2020-08-18 09:10:25', '2020-08-18 09:10:25', 0),
(40, 'SGN-BKK', 1, '2020-08-18 17:00:00', '[{\"departure\":\"SGN\",\"arrival\":\"BKK\",\"route\":\"SGN-BKK\",\"typeSchedule\":0,\"flightNumber\":\"VJ123\",\"status\":1,\"flightTime\":[\"Mon\",\"Fri\",\"Tue\",\"Sat\",\"Wed\",\"Sun\",\"Thu\"]}]', '2020-08-19 08:22:24', '2020-08-19 08:22:24', 2),
(41, 'SGN-SIN', 1, '2020-09-22 17:00:00', '[{\"departure\":\"SGN\",\"arrival\":\"SIN\",\"route\":\"SGN-SIN\",\"typeSchedule\":0,\"flightNumber\":\"VJ202\",\"status\":1,\"flightTime\":[\"Mon\",\"Fri\",\"Tue\",\"Sat\",\"Wed\",\"Sun\",\"Thu\"]}]', '2020-09-23 04:02:03', '2020-09-23 04:02:03', 2);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_copy_seal`
--

CREATE TABLE `wh_copy_seal` (
  `citypair_id` int(11) NOT NULL,
  `data_copy` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data_copy`))
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_declaration`
--

CREATE TABLE `wh_declaration` (
  `de_number` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `de_type` int(11) NOT NULL,
  `de_date_re` datetime DEFAULT NULL,
  `de_status` int(11) DEFAULT 1,
  `de_parent_number` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_content` longtext COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_extended_dispatch` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_extended_dispatch_date` datetime DEFAULT NULL,
  `de_settlement_dispatch` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_renewal_date` datetime DEFAULT NULL,
  `de_settlement_status` int(11) NOT NULL DEFAULT 0,
  `de_new_date` datetime DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_declaration`
--

INSERT INTO `wh_declaration` (`de_number`, `de_type`, `de_date_re`, `de_status`, `de_parent_number`, `de_content`, `de_extended_dispatch`, `de_extended_dispatch_date`, `de_settlement_dispatch`, `de_renewal_date`, `de_settlement_status`, `de_new_date`, `user_id`) VALUES
('0242301548', 1, '2018-03-29 00:00:00', 0, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"29/03/2019\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 6),
('101752973280', 1, '2019-11-01 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"01/11/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"04/11/2020\",\"importnumber\":\"\",\"supplier\":\"\",\"deliver\":\"\"}', '11/2020', NULL, NULL, '2020-11-04 00:00:00', 0, '2021-11-01 00:00:00', 3),
('101752973820', 1, '2017-12-08 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"08/12/2018\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"08/12/2017\",\"importnumber\":\"2017/001\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV128', NULL, '123', '2020-11-03 00:00:00', 0, '2019-12-08 00:00:00', 6),
('101927748750', 1, '2018-03-29 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"29/03/2019\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"29/03/2018\",\"importnumber\":\"2018/001\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV124', NULL, '124', '2020-11-03 00:00:00', 0, '2020-03-29 00:00:00', 6),
('101927798120', 1, '2018-03-29 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"29/03/2019\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"29/03/2018\",\"importnumber\":\"2018/002\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV126', NULL, '125', '2020-11-03 00:00:00', 0, '2020-03-29 00:00:00', 6),
('101927808400', 1, '2018-03-29 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"29/03/2019\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"29/03/2018\",\"importnumber\":\"2018/003\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV127', NULL, '126', '2020-11-03 00:00:00', 0, '2020-03-29 00:00:00', 6),
('101927814110', 1, '2018-03-29 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"29/03/2019\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"29/03/2018\",\"importnumber\":\"2018/004\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV123', NULL, '127', '2020-11-03 00:00:00', 0, '2020-03-29 00:00:00', 6),
('102471574441', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"30/01/2019\",\"importnumber\":\"2019/001\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV129', NULL, NULL, '2020-11-03 00:00:00', 0, '2021-01-30 00:00:00', 6),
('102471591721', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"30/01/2019\",\"importnumber\":\"2019/002\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV133', NULL, NULL, '2020-11-03 00:00:00', 0, '2021-01-30 00:00:00', 6),
('102471594740', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"30/01/2019\",\"importnumber\":\"2019/003\",\"supplier\":\"KPI\",\"deliver\":\"\"}', 'CV139', NULL, NULL, '2020-11-03 00:00:00', 0, '2021-01-30 00:00:00', 6),
('123456789', 2, '2019-11-08 00:00:00', 1, '101752973820', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"101752973820\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"08/11/2019\",\"exportnumber\":\"2017/002\",\"requestname\":\"\",\"rebill\":\"555\"}', NULL, NULL, NULL, NULL, 0, NULL, 6),
('301707525620', 2, '2020-11-03 00:00:00', 1, '102471591721', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"102471591721\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"03/11/2020\",\"exportnumber\":\"\",\"requestname\":\"\",\"rebill\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 3),
('556', 2, '2020-02-28 00:00:00', 1, '101927748750', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"101927748750\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"28/02/2020\",\"exportnumber\":\"2008/11\",\"requestname\":\"\",\"rebill\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 6),
('557', 2, '2020-02-28 00:00:00', 0, '101927748750', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"101927748750\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 6),
('558', 2, '2020-02-28 00:00:00', 1, '101927808400', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"101927808400\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"28/02/2020\",\"exportnumber\":\"2018/114\",\"requestname\":\"\",\"rebill\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 6),
('559', 2, '2020-02-28 00:00:00', 1, '101927814110', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"101927814110\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"28/02/2020\",\"exportnumber\":\"2018/115\",\"requestname\":\"\",\"rebill\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 6),
('560', 2, '2020-02-28 00:00:00', 1, '101927798120', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"101927798120\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"28/02/2020\",\"exportnumber\":\"2018/113\",\"requestname\":\"\",\"rebill\":\"\"}', NULL, NULL, NULL, NULL, 0, NULL, 6);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_declaration_extension`
--

CREATE TABLE `wh_declaration_extension` (
  `declaration_extension_id` varchar(255) NOT NULL,
  `declaration_number` varchar(255) NOT NULL,
  `hs_code` varchar(255) NOT NULL,
  `product_code` varchar(255) NOT NULL,
  `quantity_inventory` int(11) NOT NULL,
  `declaration_extension_created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_department`
--

CREATE TABLE `wh_department` (
  `department_id` int(11) NOT NULL,
  `department_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `department_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `department_enable` tinyint(1) NOT NULL,
  `department_created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_department`
--

INSERT INTO `wh_department` (`department_id`, `department_name`, `department_code`, `department_enable`, `department_created_at`) VALUES
(1, 'System', 'SYSS', 1, '2020-02-17 08:44:21'),
(2, 'Ancillary', 'ANCI', 1, '2020-02-17 08:44:36'),
(3, 'SCSC', 'SCSC', 1, '2020-02-17 08:44:42');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_department_permissions`
--

CREATE TABLE `wh_department_permissions` (
  `department_permission_id` int(11) NOT NULL,
  `department_permission_departmentid` int(11) DEFAULT NULL,
  `department_permission_permissionid` int(11) DEFAULT NULL,
  `department_permission_departmentcode` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `department_permission_permissioncode` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_destroy`
--

CREATE TABLE `wh_destroy` (
  `destroy_id` int(11) NOT NULL,
  `destroy_request_date` datetime NOT NULL,
  `destroy_status` int(11) DEFAULT 1,
  `destroy_user` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `destroy_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `destroy_date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='lịch sử hủy';

--
-- Đang đổ dữ liệu cho bảng `wh_destroy`
--

INSERT INTO `wh_destroy` (`destroy_id`, `destroy_request_date`, `destroy_status`, `destroy_user`, `destroy_code`, `destroy_date`) VALUES
(1, '2020-11-03 07:06:09', 1, 'tuanngoanh@vietjetair.com', 'CV900', '2020-11-03 00:00:00'),
(2, '2020-11-03 14:06:46', 1, 'admin', '2020-11/03', '2020-11-03 00:00:00'),
(3, '2020-11-18 18:14:19', 0, 'admin', 'BALA', '2020-12-01 00:00:00'),
(4, '2020-11-26 15:15:32', 0, 'tuanngoanh@vietjetair.com', 'CVhuy26112020', '2020-11-30 00:00:00');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_destroy_detail`
--

CREATE TABLE `wh_destroy_detail` (
  `destroy_detail_id` int(11) NOT NULL,
  `de_number` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `destroy_id` int(11) NOT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `destroy_detail_quantity` int(11) DEFAULT NULL,
  `destroy_detail_note` mediumtext COLLATE utf8_unicode_ci DEFAULT NULL,
  `product_price` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_destroy_detail`
--

INSERT INTO `wh_destroy_detail` (`destroy_detail_id`, `de_number`, `destroy_id`, `product_code`, `destroy_detail_quantity`, `destroy_detail_note`, `product_price`) VALUES
(1, '101752973820', 1, '04D8000001', 1, 'Hư hại', 30),
(3, '102471591721', 2, '16F1100006', 3, 'Hư hỏng, không bán được', 9),
(11, '102471594740', 4, '1596500141', 1, 'Hư Hại', 39),
(12, '101752973280', 3, '0113800502', 1, 'Hư hỏng', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_de_details`
--

CREATE TABLE `wh_de_details` (
  `dt_id` int(11) NOT NULL,
  `de_number` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `dt_quantity` int(11) NOT NULL,
  `dt_invoice_price` double DEFAULT NULL,
  `dt_invoice_value` double DEFAULT NULL,
  `dt_product_number` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `dt_own_management_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `dt_code_re_confirm` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_de_details`
--

INSERT INTO `wh_de_details` (`dt_id`, `de_number`, `product_code`, `dt_quantity`, `dt_invoice_price`, `dt_invoice_value`, `dt_product_number`, `dt_own_management_code`, `dt_code_re_confirm`) VALUES
(1, '101752973820', '04D8000001', 100, 30, 3000, '111', NULL, NULL),
(2, '101927748750', '0114400314', 40, 32.4, 1296, '113', NULL, NULL),
(3, '101927748750', '0141500255', 100, 31.2, 3120, '112', NULL, NULL),
(4, '101927798120', '0242301548', 30, 21, 630, '114', NULL, NULL),
(5, '101927808400', '0454800060', 15, 27, 405, '115', NULL, NULL),
(6, '101927814110', '15F2800006', 6, 29.4, 176.39999999999998, '117', NULL, NULL),
(7, '101927814110', '1594400008', 2, 119.4, 238.8, '116', NULL, NULL),
(8, '102471574441', '0153600039', 22, 49.2, 1082.4, '121', NULL, NULL),
(9, '102471574441', '0153600037', 22, 36.6, 805.2, '120', NULL, NULL),
(10, '102471574441', '0113800502', 33, 36.6, 1207.8, '119', NULL, NULL),
(11, '102471574441', '0113800503', 40, 30, 1200, '118', NULL, NULL),
(12, '102471591721', '16F1100006', 15, 9, 135, '127', NULL, NULL),
(13, '102471591721', '04D8000001', 10, 30, 300, '126', NULL, NULL),
(14, '102471591721', '0454800059', 5, 24, 120, '125', NULL, NULL),
(15, '102471591721', '0454800055', 24, 19.2, 460.79999999999995, '124', NULL, NULL),
(16, '102471591721', '0452100963', 10, 51, 510, '123', NULL, NULL),
(17, '102471591721', '0454800035', 20, 33, 660, '122', NULL, NULL),
(18, '102471594740', '15G0400001', 18, 33, 594, '132', NULL, NULL),
(19, '102471594740', '1596500141', 15, 39, 585, '131', NULL, NULL),
(20, '102471594740', '15D4500002', 6, 107.4, 644.4000000000001, '130', NULL, NULL),
(21, '102471594740', '15G0300001', 25, 33, 825, '129', NULL, NULL),
(22, '102471594740', '15G6000002', 6, 35.4, 212.39999999999998, '128', NULL, NULL),
(23, '123456789', '04D8000001', 99, 30, 2970, '111', NULL, NULL),
(24, '556', '0114400314', 39, 32.4, 1263.6, '113', NULL, NULL),
(25, '556', '0141500255', 99, 31.2, 3088.7999999999997, '112', NULL, NULL),
(26, '560', '0242301548', 29, 21, 609, '114', NULL, NULL),
(27, '558', '0454800060', 14, 27, 378, '115', NULL, NULL),
(28, '559', '15F2800006', 6, 29.4, 176.39999999999998, '117', NULL, NULL),
(29, '559', '1594400008', 2, 119.4, 238.8, '116', NULL, NULL),
(31, '301707525620', '16F1100006', 5, 9, 45, '127', NULL, NULL),
(32, '101752973280', '0113800502', 10, 1, 10, '33030000', NULL, NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_exchangerates`
--

CREATE TABLE `wh_exchangerates` (
  `exchangerate_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `exchangerate_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `exchangerate_rate` double NOT NULL,
  `exchangerate_status` int(1) NOT NULL DEFAULT 1 COMMENT '1 : hiển thị, 0: ẩn',
  `exchangerate_order` int(10) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_exchangerates`
--

INSERT INTO `wh_exchangerates` (`exchangerate_id`, `exchangerate_code`, `exchangerate_rate`, `exchangerate_status`, `exchangerate_order`) VALUES
('6f6e98fb-b40b-4056-b955-b592507b97cf', 'VND', 23000, 1, 2),
('c23b0d57-c0de-4666-bf7d-7c100e0ba718', 'USD', 1, 1, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_file_upload`
--

CREATE TABLE `wh_file_upload` (
  `file_id` int(11) NOT NULL,
  `file_type` int(11) DEFAULT 1,
  `file_path` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL,
  `file_created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `file_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_file_upload`
--

INSERT INTO `wh_file_upload` (`file_id`, `file_type`, `file_path`, `user_id`, `file_created_at`, `file_name`) VALUES
(1, 1, 'Seal/DcThCfo_TemplateHangDieuChuyen9.xlsx', 6, '2020-11-03 01:23:36', 'DcThCfo_TemplateHangDieuChuyen9.xlsx'),
(2, 2, 'Sell/xkuDniV_TemplateDuLieuBanHang11.xlsx', 6, '2020-11-03 01:39:11', 'xkuDniV_TemplateDuLieuBanHang11.xlsx'),
(3, 1, 'Seal/KUnQmsn_TemplateHangDieuChuyen.xlsx', 3, '2020-11-06 10:40:54', 'KUnQmsn_TemplateHangDieuChuyen.xlsx'),
(4, 1, 'Seal/smUupGI_TemplateHangDieuChuyen.xlsx', 6, '2020-11-26 08:46:50', 'smUupGI_TemplateHangDieuChuyen.xlsx');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_inventory`
--

CREATE TABLE `wh_inventory` (
  `in_id` int(11) NOT NULL,
  `de_number` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `in_quantity` int(11) NOT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `settlement_date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_inventory`
--

INSERT INTO `wh_inventory` (`in_id`, `de_number`, `in_quantity`, `product_code`, `settlement_date`) VALUES
(1, '101752973820', 0, '04D8000001', '2020-11-06 17:50:44'),
(2, '101927748750', 0, '0114400314', '2020-11-06 15:59:35'),
(3, '101927748750', 0, '0141500255', '2020-11-06 15:59:35'),
(4, '101927798120', 0, '0242301548', '2020-11-06 15:59:35'),
(5, '101927808400', 0, '0454800060', '2020-11-06 15:59:36'),
(6, '101927814110', 0, '15F2800006', '2020-11-06 15:59:36'),
(7, '101927814110', 0, '1594400008', '2020-11-06 15:59:36'),
(8, '102471574441', 20, '0153600039', NULL),
(9, '102471574441', 20, '0153600037', NULL),
(10, '102471574441', 31, '0113800502', NULL),
(11, '102471574441', 38, '0113800503', NULL),
(12, '102471591721', 4, '16F1100006', NULL),
(13, '102471591721', 7, '04D8000001', NULL),
(14, '102471591721', 5, '0454800059', NULL),
(15, '102471591721', 22, '0454800055', NULL),
(16, '102471591721', 8, '0452100963', NULL),
(17, '102471591721', 18, '0454800035', NULL),
(18, '102471594740', 17, '15G0400001', NULL),
(19, '102471594740', 14, '1596500141', NULL),
(20, '102471594740', 6, '15D4500002', NULL),
(21, '102471594740', 23, '15G0300001', NULL),
(22, '102471594740', 5, '15G6000002', NULL),
(23, '101752973280', 10, '0113800502', NULL);

--
-- Bẫy `wh_inventory`
--
DELIMITER $$
CREATE TRIGGER `update_settlement_date` BEFORE UPDATE ON `wh_inventory` FOR EACH ROW IF new.in_quantity > 0 THEN
    	 SET new.settlement_date = NULL;
    END IF
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_menus`
--

CREATE TABLE `wh_menus` (
  `menu_id` int(11) NOT NULL,
  `menu_name` varchar(255) NOT NULL,
  `menu_start_time` datetime NOT NULL,
  `menu_stop_time` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Đang đổ dữ liệu cho bảng `wh_menus`
--

INSERT INTO `wh_menus` (`menu_id`, `menu_name`, `menu_start_time`, `menu_stop_time`) VALUES
(1, 'Default', '2020-10-30 16:49:28', '2020-10-30 16:49:28');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_menu_details`
--

CREATE TABLE `wh_menu_details` (
  `menu_detail_id` int(10) UNSIGNED NOT NULL,
  `menu_id` int(11) DEFAULT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `menu_detail_parlever` int(11) DEFAULT NULL,
  `menu_detail_order` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_menu_details`
--

INSERT INTO `wh_menu_details` (`menu_detail_id`, `menu_id`, `product_code`, `menu_detail_parlever`, `menu_detail_order`) VALUES
(1, 1, '0113800502', 2, 2),
(2, 1, '0113800503', 2, 3),
(5, 1, '0153600037', 1, 6),
(6, 1, '0153600039', 1, 7),
(8, 1, '0452100963', 1, 9),
(9, 1, '0454800035', 1, 10),
(10, 1, '0454800055', 1, 11),
(11, 1, '0454800059', 1, 12),
(13, 1, '04D8000001', 1, 14),
(15, 1, '1596500141', 1, 16),
(16, 1, '15D4500002', 1, 17),
(18, 1, '15G0300001', 1, 19),
(19, 1, '15G0400001', 1, 20),
(20, 1, '15G6000002', 1, 21),
(21, 1, '16F1100006', 1, 22);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_modules`
--

CREATE TABLE `wh_modules` (
  `module_id` int(11) NOT NULL,
  `module_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `module_created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `module_enable` bit(1) DEFAULT NULL,
  `module_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_modules`
--

INSERT INTO `wh_modules` (`module_id`, `module_name`, `module_created_at`, `module_enable`, `module_code`) VALUES
(9, 'Quản lý tờ khai', '2020-02-17 08:23:58', b'1', 'QLTK'),
(10, 'Quản lý điều chuyển', '2020-02-17 08:24:04', b'1', 'QLDC'),
(11, 'Báo cáo', '2020-02-17 08:24:16', b'1', 'BACA'),
(12, 'Danh mục sản phẩm', '2020-02-17 08:24:20', b'1', 'DMSP'),
(13, 'Quản lý chuyến bay', '2020-02-17 08:24:25', b'1', 'QLCB'),
(14, 'Quản lý người dùng', '2020-02-17 08:24:30', b'1', 'QLND');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_permissions`
--

CREATE TABLE `wh_permissions` (
  `permission_id` int(11) NOT NULL,
  `permission_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `permission_created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `permission_enable` tinyint(1) NOT NULL,
  `permission_module_id` int(11) DEFAULT NULL,
  `permission_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_permissions`
--

INSERT INTO `wh_permissions` (`permission_id`, `permission_code`, `permission_created_at`, `permission_enable`, `permission_module_id`, `permission_name`) VALUES
(1, 'QLTK_DSTK', '2020-02-17 08:16:58', 1, 9, 'Danh sách tờ khai'),
(2, 'QLTK_TTKN', '2020-02-17 08:17:14', 1, 9, 'Tạo tờ khai nhập'),
(3, 'QLTK_TTKX', '2020-02-17 08:17:20', 1, 9, 'Tạo tờ khai xuất'),
(4, 'QLDC_XDDC', '2020-02-17 08:17:57', 1, 10, 'Xem danh sách điều chuyển'),
(5, 'QLDC_NDDC', '2020-02-17 08:18:07', 1, 10, 'Nhập dữ liệu điều chuyển'),
(6, 'QLDC_NDLB', '2020-02-17 08:18:23', 1, 10, 'Nhập dữ liệu bán'),
(7, 'QLDC_LSHU', '2020-02-17 08:18:36', 1, 10, 'Lịch sử đề xuất hủy'),
(8, 'QLDC_TDXH', '2020-02-17 08:18:49', 1, 10, 'Tạo đề xuất hủy mới'),
(9, 'QLDC_NHDB', '2020-02-17 08:19:03', 1, 10, 'Nhập hóa đơn bán hàng'),
(10, 'BACA_CTHX', '2020-02-17 08:19:27', 1, 11, 'Báo cáo chi tiết hàng xuất'),
(11, 'BACA_CTHN', '2020-02-17 08:19:36', 1, 11, 'Báo cáo chi tiết hàng nhập'),
(12, 'BACA_BCTK', '2020-02-17 08:19:51', 1, 11, 'Báo cáo tồn kho'),
(13, 'BACA_BK05', '2020-02-17 08:19:59', 1, 11, 'Bảng kê 05'),
(14, 'BACA_QTKD', '2020-02-17 08:20:19', 1, 11, 'Báo cáo quyết toán kinh doanh'),
(15, 'BACA_TTTK', '2020-02-17 08:20:35', 1, 11, 'Báo cáo theo dõi tình trạng tờ khai'),
(16, 'DMSP_XDSP', '2020-02-17 08:20:54', 1, 12, 'Xem danh mục sản phẩm'),
(17, 'DMSP_TSPM', '2020-02-17 08:21:05', 1, 12, 'Thêm sản phẩm mới'),
(18, 'DMSP_CNSP', '2020-02-17 08:21:15', 1, 12, 'Cập nhật thông tin sản phẩm'),
(19, 'DMSP_XOSP', '2020-02-17 08:21:33', 1, 12, 'Xóa sản phẩm'),
(20, 'DMSP_TMBH', '2020-02-17 08:21:40', 1, 12, 'Tạo menu bán hàng'),
(21, 'QLCB_DSCB', '2020-02-17 08:22:00', 1, 13, 'Danh sách chuyến bay'),
(22, 'QLCB_TDBM', '2020-02-17 08:22:08', 1, 13, 'Thêm đường bay mới'),
(23, 'QLCB_CNCB', '2020-02-17 08:22:22', 1, 13, 'Cập nhật thông tin các chuyến bay'),
(24, 'QLND_DSND', '2020-02-17 08:22:49', 1, 14, 'Danh sách người dùng'),
(25, 'QLND_TNDM', '2020-02-17 08:23:03', 1, 14, 'Thêm người dùng mới'),
(26, 'QLND_CNND', '2020-02-17 08:23:15', 1, 14, 'Cập nhật thông tin người dùng'),
(27, 'QLND_DSBP', '2020-02-17 08:23:26', 1, 14, 'Danh sách bộ phận'),
(28, 'QLND_PQCN', '2020-02-17 08:23:37', 1, 14, 'Phân quyền chức năng'),
(30, 'QLDC_DCHN', '2020-02-19 08:56:35', 1, 10, 'Xem điều chuyển hàng ngày'),
(31, 'QLDC_CNHN', '2020-02-19 08:56:35', 1, 10, 'Cập nhập điều chuyển hằng ngày'),
(32, 'DMSP_IPSP', '2020-02-19 10:47:23', 1, 12, 'Nhập dữ liệu sản phẩm'),
(33, 'QLND_TMBP', '2020-02-20 10:48:09', 1, 14, 'Thêm mới bộ phận'),
(34, 'QLND_DSBP', '2020-02-20 10:48:09', 1, 14, 'Cập nhật bộ phận'),
(35, 'QLND_XOBP', '2020-02-20 10:48:33', 1, 14, 'Xóa bộ phận'),
(36, 'QLND_XOND', '2020-02-20 10:58:22', 1, 14, 'Xóa người dùng');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_products`
--

CREATE TABLE `wh_products` (
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `product_name` text COLLATE utf8_unicode_ci NOT NULL,
  `product_unit` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `product_type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `product_parlevel` int(10) UNSIGNED NOT NULL,
  `product_status` tinyint(3) UNSIGNED NOT NULL DEFAULT 1,
  `product_createdat` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_products`
--

INSERT INTO `wh_products` (`product_code`, `product_name`, `product_unit`, `product_type`, `product_parlevel`, `product_status`, `product_createdat`) VALUES
('0113800502', 'Bộ nước hoa nữ hiệu Bvlgari (Bvlgari The Women\'s Gift Collection 5mlx5), 0113800502', 'Pcs', 'Nước hoa', 1, 1, '2020-10-30 08:45:46'),
('0113800503', 'Bộ nước hoa nam hiệu Bvlgari (Bvlgari The Men\'s Gift Collection 5mlx5), 0113800503', 'Pcs', 'Nước hoa', 1, 1, '2020-10-30 08:45:46'),
('0114400314', 'Bộ nước hoa nữ hiệu Calvin Klein ( Calvin Klein Miniatures Individually Packed Coffret for Her, 10mlx2, 5mlx2, 4ml), 0114400314', 'Pcs', 'Nước hoa', 1, 1, '2020-10-30 08:45:46'),
('0141500255', 'Bộ nước hoa hiệu Lancome ( Lancome Best Of Fragrances boxed 5mlx3, 7.5ml, 4ml), 0141500255', 'Pcs', 'Nước hoa', 1, 1, '2020-10-30 08:45:46'),
('0153600037', 'Nước hoa hiệu Paco Rabanne ( Paco Rabanne 1 Million EDT 50ml ), 0153600037', 'Pcs', 'Nước hoa', 1, 1, '2020-10-30 08:45:46'),
('0153600039', 'Nước hoa nữ hiệu Paco Rabanne ( Paco Rabanne Lady Million EDP 50ml ), 0153600039', 'Pcs', 'Nước hoa', 1, 1, '2020-10-30 08:45:46'),
('0242301548', 'Mỹ phẩm hiệu L\'Oreal (Bộ 3 cây son môi L\'Oreal CC Balm Genius by Balm Caresse - Colour Shades 3gx3), 0242301548', 'Pcs', 'Mỹ phẩm', 1, 1, '2020-10-30 08:45:46'),
('0452100963', 'Vòng cổ \"Coral\" hiệu Pica Lela kèm hoa tai (Pica LéLa Australia \"Coral\" 18K Rose Gold Plated Necklace WithCats Eye Centre Stone), 0452100963', 'Pcs', 'Trang sức', 1, 1, '2020-10-30 08:45:46'),
('0454800035', 'Bộ vòng tay hiệu Pierre Cardin ( Pierre Cardin Bangle Set ), 0454800035', 'Pcs', 'Trang sức', 2, 1, '2020-10-30 08:45:46'),
('0454800055', 'Bộ 9 đôi hoa tai hiệu Pierre Cardin (Pierre Cardin Earring Set), 0454800055', 'Pcs', 'Trang sức', 1, 1, '2020-10-30 08:45:46'),
('0454800059', 'Bộ hoa tai ngọc trai hiệu Pierre Cardin (Pierre Cardin Pearl Earring Set), 0454800059', 'Pcs', 'Trang sức', 1, 1, '2020-10-30 08:45:46'),
('0454800060', 'Bộ dây chuyền kèm bông tai hiệu Pierre Cardin ( Pierre Cardin Jewellery Set, set of 4 ), 0454800060', 'Pcs', 'Trang sức', 1, 1, '2020-10-30 08:45:46'),
('04D8000001', 'Vòng đeo tay kèm 1 đôi bông tai hiệu Joia  ( Joia De Majorca Pearl Bracelet With Free Earrings ), 04D8000001', 'Pcs', 'Trang sức', 1, 1, '2020-10-30 08:45:46'),
('1594400008', 'Đồng hồ hiệu Hanowa \"SEALION X\" (Swiss Military Hanowa \"SEALION X\" Gents Watch), tặng kèm dây đeo, 1594400008', 'Pcs', 'Trang sức', 1, 1, '2020-10-30 08:45:46'),
('1596500141', 'Đồng hồ nam hiệu Sekonda ( Sekonda Gents Sports Watch ), 1596500141', 'Pcs', 'Đồng hồ', 1, 1, '2020-10-30 08:45:46'),
('15D4500002', 'Đồng hồ nam hiệu Kenneth Cole (Kenneth Cole \"Automatic\" Gents Watch), 15D4500002', 'Pcs', 'Đồng hồ', 1, 1, '2020-10-30 08:45:46'),
('15F2800006', 'Đồng hồ nữ hiệu Temptation (Temptation Ladies Watch + 5 Straps), kèm 5 dây đeo, 15F2800006', 'Pcs', 'Đồng hồ', 1, 1, '2020-10-30 08:45:46'),
('15G0300001', 'Đồng hồ nam hiệu Geoffrey Beene (Geoffrey Beene Manhattan Collection Gents Watch) kèm dây da, 15G0300001', 'Pcs', 'Đồng hồ', 1, 1, '2020-10-30 08:45:46'),
('15G0400001', 'Đồng hồ nữ kèm vòng tay hiệu Ellen Tracy (Ellen Tracy Elegance Collection Ladies Gold Tone Watch), 15G0400001', 'Pcs', 'Đồng hồ', 1, 1, '2020-10-30 08:45:46'),
('15G6000002', 'Bộ đồng hồ kèm 2 vòng tay hiệu Cristian Cole (Cristian Cole Glittering Watch & Bangles Set), 15G6000002', 'Pcs', 'Đồng hồ', 1, 1, '2020-10-30 08:45:46'),
('16F1100006', 'Gậy \"tự sướng\" Click Stick (Thumbs Up! Click Stick), hiệu Thumbs Up, bằng nhôm và nhựa, 16F1100006', 'Pcs', 'Phụ kiện', 2, 1, '2020-10-30 08:45:46');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_seal`
--

CREATE TABLE `wh_seal` (
  `se_number` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `se_flightnumber` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `se_flightdate` datetime NOT NULL,
  `se_acreg` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `se_status` int(11) DEFAULT 1,
  `se_export_date` datetime DEFAULT NULL,
  `se_return` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `se_import_date` datetime DEFAULT NULL,
  `se_citypair_id` int(11) DEFAULT NULL,
  `se_route` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_seal`
--

INSERT INTO `wh_seal` (`se_number`, `se_flightnumber`, `se_flightdate`, `se_acreg`, `se_status`, `se_export_date`, `se_return`, `se_import_date`, `se_citypair_id`, `se_route`) VALUES
('1000', 'VJ801, VJ802', '2019-02-04 00:00:00', 'VNA655', 2, '2020-11-03 08:28:17', NULL, '2020-11-03 08:39:50', NULL, 'SGN-BKK'),
('1001', 'VJ808, VJ809', '2019-02-04 00:00:00', 'VNA659', 2, '2020-11-03 08:29:46', NULL, '2020-11-03 08:39:58', NULL, 'SGN-HKT'),
('1002', 'VJ811, VJ812', '2019-02-04 00:00:00', 'VNA669', 2, '2020-11-03 08:31:00', NULL, '2020-11-03 08:40:09', NULL, 'SGN-SIN'),
('1003', 'VJ940, VJ941', '2019-02-04 00:00:00', 'VNA678', 2, '2020-11-03 08:32:35', NULL, '2020-11-03 08:40:13', NULL, 'HAN-TPE'),
('1004', 'VJ828', '2019-02-04 00:00:00', 'VNA708', 2, '2020-11-03 08:34:10', NULL, '2020-11-03 08:40:16', NULL, 'SGN-KIX'),
('1005', 'VJ829', '2019-02-04 00:00:00', 'VNA708', 2, '2020-11-03 08:34:32', NULL, '2020-11-03 08:40:21', NULL, 'SGN-KIX'),
('1006', 'VJ860', '2019-02-04 00:00:00', 'VNA777', 2, '2020-11-03 08:35:04', NULL, '2020-11-03 08:40:25', NULL, 'SGN-ICN'),
('1007', 'VJ861', '2019-02-04 00:00:00', 'VNA777', 2, '2020-11-03 08:35:25', NULL, '2020-11-03 08:40:33', NULL, 'SGN-ICN'),
('1008', 'VJ932', '2019-02-04 00:00:00', 'VNA998', 2, '2020-11-03 08:35:50', NULL, '2020-11-03 08:40:39', NULL, 'HAN-NRT'),
('1009', 'VJ933', '2019-02-04 00:00:00', 'VNA998', 2, '2020-11-03 08:36:24', NULL, '2020-11-03 08:40:44', NULL, 'HAN-NRT'),
('11032019', 'VJ840, VJ841', '2020-11-03 00:00:00', 'VNA322', 2, '2020-11-03 14:00:28', '11032020', '2020-11-03 14:04:55', 8, 'SGN-TPE'),
('123', 'VJ801, VJ802', '2020-11-06 00:00:00', 'VNA655', 2, '2020-11-06 15:19:13', '321', '2020-11-06 15:59:35', 1, 'SGN-BKK'),
('123abc', 'VJ829', '2021-02-24 00:00:00', 'vna655', 2, '2021-02-24 17:00:00', '321cba', '2021-02-24 13:47:48', 6, 'SGN-KIX'),
('147', 'VJ860', '2020-11-06 00:00:00', 'VNA777', 2, '2020-11-06 15:50:54', '741', '2020-11-06 17:46:26', 11, 'SGN-ICN'),
('159', 'VJ940, VJ941', '2020-11-06 00:00:00', 'VNA678', 2, '2020-11-06 15:38:11', '951', '2020-11-06 17:08:19', 27, 'HAN-TPE'),
('258', 'VJ829', '2020-11-06 00:00:00', 'VNA708', 2, '2020-11-06 15:50:37', '852', '2020-11-06 17:48:03', 6, 'SGN-KIX'),
('357', 'VJ828', '2020-11-06 00:00:00', 'VNA708', 2, '2020-11-06 15:50:27', '753', '2020-11-06 17:38:30', 6, 'SGN-KIX'),
('369', 'VJ932', '2020-11-06 00:00:00', 'VNA998', 2, '2020-11-06 15:51:09', '963', '2020-11-06 17:50:24', 24, 'HAN-NRT'),
('456', 'VJ808, VJ809', '2020-11-06 00:00:00', 'VNA659', 2, '2020-11-06 15:36:17', '654', '2020-11-06 17:01:37', 2, 'SGN-HKT'),
('456def', 'VJ828', '2021-02-24 00:00:00', 'vna655', 2, '2021-03-01 06:53:07', '654fed', '2021-03-01 13:54:53', 6, 'SGN-KIX'),
('708', 'VJ933', '2020-11-06 00:00:00', 'VNA998', 2, '2020-11-06 15:51:16', '807', '2020-11-06 17:50:43', 24, 'HAN-NRT'),
('789', 'VJ811, VJ812', '2020-11-06 00:00:00', 'VNA669', 2, '2020-11-06 15:38:04', '987', '2020-11-06 17:05:35', 3, 'SGN-SIN'),
('9988', 'VJ861', '2020-11-06 00:00:00', 'VNA777', 2, '2020-11-06 15:51:01', '8899', '2020-11-06 17:49:01', 11, 'SGN-ICN'),
('a123', 'VJ801, VJ802', '2020-11-26 00:00:00', 'vna123', 0, NULL, '456a', NULL, 1, 'SGN-BKK'),
('AB123', 'VJ811, VJ812', '2020-11-26 00:00:00', 'VNA699', 0, NULL, 'AB321', NULL, 3, 'SGN-SIN'),
('ab2020', 'VJ801, VJ802', '2020-12-15 00:00:00', 'VNA666', 0, NULL, 'cd2020', NULL, 1, 'SGN-BKK'),
('AB456', 'VJ829', '2020-11-26 00:00:00', 'VNA659', 0, NULL, 'AB654', NULL, 6, 'SGN-KIX'),
('AB789', 'VJ828', '2020-11-26 00:00:00', 'VNA659', 0, NULL, 'AB987', NULL, 6, 'SGN-KIX');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_seal_detail`
--

CREATE TABLE `wh_seal_detail` (
  `sealdetail_id` int(11) NOT NULL,
  `seal_number` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `de_number` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `sealdetail_quantity_sell` int(11) DEFAULT NULL,
  `sealdetail_quantity_export` int(11) DEFAULT NULL,
  `sealdetail_quantity_inventory` int(11) DEFAULT NULL COMMENT 'số lượng tồn',
  `sealdetail_quantity_real` int(11) DEFAULT NULL COMMENT 'số lượng tồn thực tế'
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_seal_detail`
--

INSERT INTO `wh_seal_detail` (`sealdetail_id`, `seal_number`, `de_number`, `product_code`, `sealdetail_quantity_sell`, `sealdetail_quantity_export`, `sealdetail_quantity_inventory`, `sealdetail_quantity_real`) VALUES
(1, '1000', '102471574441', '0113800502', 0, 2, 2, 2),
(2, '1000', '102471594740', '15G0400001', 0, 1, 1, 1),
(3, '1000', '102471594740', '15G0300001', 0, 1, 1, 1),
(4, '1000', '101927814110', '15F2800006', 0, 1, 1, 1),
(5, '1000', '102471594740', '15D4500002', 0, 1, 1, 1),
(6, '1000', '102471594740', '1596500141', 0, 1, 1, 1),
(7, '1000', '101927814110', '1594400008', 0, 1, 1, 1),
(8, '1000', '102471591721', '04D8000001', 1, 1, 0, 0),
(9, '1000', '101927808400', '0454800060', 0, 1, 1, 1),
(10, '1000', '102471594740', '15G6000002', 0, 1, 1, 1),
(11, '1000', '102471591721', '0454800059', 0, 1, 1, 1),
(12, '1000', '102471591721', '0454800035', 0, 1, 1, 1),
(13, '1000', '102471591721', '0452100963', 1, 1, 0, 0),
(14, '1000', '101927798120', '0242301548', 0, 1, 1, 1),
(15, '1000', '102471574441', '0153600039', 0, 1, 1, 1),
(16, '1000', '102471574441', '0153600037', 0, 1, 1, 1),
(17, '1000', '101927748750', '0141500255', 1, 2, 1, 1),
(18, '1000', '101927748750', '0114400314', 1, 1, 0, 0),
(19, '1000', '102471574441', '0113800503', 0, 2, 2, 2),
(20, '1000', '102471591721', '0454800055', 0, 1, 1, 1),
(21, '1000', '102471591721', '16F1100006', 0, 1, 1, 1),
(22, '1001', '102471591721', '0452100963', 0, 1, 1, 1),
(23, '1001', '102471574441', '0113800502', 0, 2, 2, 2),
(24, '1001', '101927748750', '0141500255', 0, 2, 2, 2),
(25, '1001', '102471574441', '0153600037', 0, 1, 1, 1),
(26, '1001', '102471574441', '0153600039', 0, 1, 1, 1),
(27, '1001', '102471594740', '15G0400001', 0, 1, 1, 1),
(28, '1001', '102471591721', '16F1100006', 0, 1, 1, 1),
(29, '1001', '102471594740', '15G6000002', 0, 1, 1, 1),
(30, '1001', '102471594740', '15G0300001', 0, 1, 1, 1),
(31, '1001', '102471574441', '0113800503', 0, 2, 2, 2),
(32, '1001', '101927798120', '0242301548', 1, 1, 0, 0),
(33, '1001', '102471594740', '15D4500002', 0, 1, 1, 1),
(34, '1001', '102471594740', '1596500141', 0, 1, 1, 1),
(35, '1001', '101927814110', '1594400008', 0, 1, 1, 1),
(36, '1001', '102471591721', '04D8000001', 0, 1, 1, 1),
(37, '1001', '101927808400', '0454800060', 1, 1, 0, 0),
(38, '1001', '102471591721', '0454800059', 0, 1, 1, 1),
(39, '1001', '102471591721', '0454800055', 0, 1, 1, 1),
(40, '1001', '102471591721', '0454800035', 0, 1, 1, 1),
(41, '1001', '101927814110', '15F2800006', 0, 1, 1, 1),
(42, '1001', '101927748750', '0114400314', 0, 1, 1, 1),
(43, '1002', '102471574441', '0113800502', 1, 1, 0, 0),
(44, '1002', '102471594740', '15G6000002', 0, 1, 1, 1),
(45, '1002', '102471594740', '15G0400001', 0, 1, 1, 1),
(46, '1002', '102471594740', '15G0300001', 0, 1, 1, 1),
(47, '1002', '101927814110', '15F2800006', 0, 1, 1, 1),
(48, '1002', '102471594740', '15D4500002', 0, 1, 1, 1),
(49, '1002', '102471594740', '1596500141', 0, 1, 1, 1),
(50, '1002', '101927814110', '1594400008', 0, 0, 0, 0),
(51, '1002', '102471591721', '04D8000001', 0, 1, 1, 1),
(52, '1002', '102471591721', '16F1100006', 0, 1, 1, 1),
(53, '1002', '101927808400', '0454800060', 0, 1, 1, 1),
(54, '1002', '102471591721', '0454800035', 0, 1, 1, 1),
(55, '1002', '102471591721', '0452100963', 0, 1, 1, 1),
(56, '1002', '101927798120', '0242301548', 0, 1, 1, 1),
(57, '1002', '102471574441', '0153600039', 0, 1, 1, 1),
(58, '1002', '102471574441', '0153600037', 0, 1, 1, 1),
(59, '1002', '101927748750', '0141500255', 0, 1, 1, 1),
(60, '1002', '101927748750', '0114400314', 0, 1, 1, 1),
(61, '1002', '102471574441', '0113800503', 1, 1, 0, 0),
(62, '1002', '102471591721', '0454800055', 0, 1, 1, 1),
(63, '1002', '102471591721', '0454800059', 0, 1, 1, 1),
(64, '1003', '102471594740', '15G0400001', 0, 1, 1, 1),
(65, '1003', '101927808400', '0454800060', 0, 1, 1, 1),
(66, '1003', '102471591721', '0454800059', 0, 1, 1, 1),
(67, '1003', '102471591721', '0454800055', 0, 1, 1, 1),
(68, '1003', '102471591721', '0454800035', 0, 1, 1, 1),
(69, '1003', '102471591721', '0452100963', 0, 1, 1, 1),
(70, '1003', '101927798120', '0242301548', 0, 1, 1, 1),
(71, '1003', '102471574441', '0153600039', 0, 1, 1, 1),
(72, '1003', '102471574441', '0153600037', 0, 1, 1, 1),
(73, '1003', '102471591721', '04D8000001', 0, 1, 1, 1),
(74, '1003', '101927748750', '0141500255', 0, 2, 2, 2),
(75, '1003', '102471574441', '0113800503', 0, 2, 2, 2),
(76, '1003', '102471574441', '0113800502', 0, 2, 2, 2),
(77, '1003', '102471594740', '1596500141', 0, 1, 1, 1),
(78, '1003', '102471594740', '15D4500002', 0, 1, 1, 1),
(79, '1003', '101927814110', '15F2800006', 0, 1, 1, 1),
(80, '1003', '102471594740', '15G0300001', 0, 1, 1, 1),
(81, '1003', '102471591721', '16F1100006', 0, 1, 1, 1),
(82, '1003', '102471594740', '15G6000002', 0, 1, 1, 1),
(83, '1003', '101927748750', '0114400314', 0, 1, 1, 1),
(84, '1003', '101927814110', '1594400008', 0, 0, 0, 0),
(85, '1004', '102471591721', '16F1100006', 0, 1, 1, 1),
(86, '1004', '102471594740', '15G0300001', 0, 1, 1, 1),
(87, '1004', '101927814110', '15F2800006', 0, 1, 1, 1),
(88, '1004', '102471594740', '15D4500002', 0, 1, 1, 1),
(89, '1004', '102471594740', '1596500141', 0, 1, 1, 1),
(90, '1004', '101927814110', '1594400008', 0, 0, 0, 0),
(91, '1004', '102471591721', '04D8000001', 0, 1, 1, 1),
(92, '1004', '101927808400', '0454800060', 0, 1, 1, 1),
(93, '1004', '102471591721', '0454800059', 0, 1, 1, 1),
(94, '1004', '102471594740', '15G0400001', 0, 1, 1, 1),
(95, '1004', '102471591721', '0454800055', 0, 1, 1, 1),
(96, '1004', '101927798120', '0242301548', 0, 1, 1, 1),
(97, '1004', '102471574441', '0153600039', 0, 1, 1, 1),
(98, '1004', '102471574441', '0153600037', 1, 1, 0, 0),
(99, '1004', '101927748750', '0141500255', 0, 1, 1, 1),
(100, '1004', '101927748750', '0114400314', 0, 1, 1, 1),
(101, '1004', '102471574441', '0113800503', 0, 1, 1, 1),
(102, '1004', '102471574441', '0113800502', 0, 1, 1, 1),
(103, '1004', '102471594740', '15G6000002', 0, 1, 1, 1),
(104, '1004', '102471591721', '0452100963', 0, 1, 1, 1),
(105, '1004', '102471591721', '0454800035', 0, 1, 1, 1),
(106, '1005', '101927748750', '0114400314', 0, 1, 1, 1),
(107, '1005', '101927814110', '1594400008', 0, 0, 0, 0),
(108, '1005', '102471591721', '16F1100006', 0, 1, 1, 1),
(109, '1005', '102471594740', '15G6000002', 0, 1, 1, 1),
(110, '1005', '102471594740', '15G0400001', 0, 1, 1, 1),
(111, '1005', '102471594740', '15G0300001', 0, 1, 1, 1),
(112, '1005', '101927814110', '15F2800006', 0, 1, 1, 1),
(113, '1005', '102471594740', '15D4500002', 0, 1, 1, 1),
(114, '1005', '102471594740', '1596500141', 0, 1, 1, 1),
(115, '1005', '102471574441', '0113800503', 0, 1, 1, 1),
(116, '1005', '102471591721', '04D8000001', 0, 1, 1, 1),
(117, '1005', '102471591721', '0454800059', 0, 0, 0, 0),
(118, '1005', '102471591721', '0454800055', 0, 1, 1, 1),
(119, '1005', '102471591721', '0454800035', 0, 1, 1, 1),
(120, '1005', '102471591721', '0452100963', 0, 1, 1, 1),
(121, '1005', '101927798120', '0242301548', 0, 1, 1, 1),
(122, '1005', '102471574441', '0153600039', 1, 1, 0, 0),
(123, '1005', '102471574441', '0153600037', 0, 1, 1, 1),
(124, '1005', '101927748750', '0141500255', 0, 1, 1, 1),
(125, '1005', '101927808400', '0454800060', 0, 1, 1, 1),
(126, '1005', '102471574441', '0113800502', 0, 1, 1, 1),
(127, '1006', '102471591721', '0452100963', 0, 1, 1, 1),
(128, '1006', '102471574441', '0113800502', 0, 1, 1, 1),
(129, '1006', '102471574441', '0153600037', 0, 1, 1, 1),
(130, '1006', '101927748750', '0141500255', 0, 1, 1, 1),
(131, '1006', '102471574441', '0153600039', 0, 1, 1, 1),
(132, '1006', '102471594740', '15G0400001', 0, 1, 1, 1),
(133, '1006', '102471591721', '16F1100006', 0, 1, 1, 1),
(134, '1006', '102471594740', '15G6000002', 0, 0, 0, 0),
(135, '1006', '102471594740', '15G0300001', 0, 1, 1, 1),
(136, '1006', '102471574441', '0113800503', 0, 1, 1, 1),
(137, '1006', '101927798120', '0242301548', 0, 1, 1, 1),
(138, '1006', '102471594740', '15D4500002', 0, 0, 0, 0),
(139, '1006', '102471594740', '1596500141', 0, 1, 1, 1),
(140, '1006', '101927814110', '1594400008', 0, 0, 0, 0),
(141, '1006', '102471591721', '04D8000001', 0, 1, 1, 1),
(142, '1006', '101927808400', '0454800060', 0, 1, 1, 1),
(143, '1006', '102471591721', '0454800059', 0, 0, 0, 0),
(144, '1006', '102471591721', '0454800055', 0, 1, 1, 1),
(145, '1006', '102471591721', '0454800035', 1, 1, 0, 0),
(146, '1006', '101927814110', '15F2800006', 0, 0, 0, 0),
(147, '1006', '101927748750', '0114400314', 0, 1, 1, 1),
(148, '1007', '102471574441', '0113800502', 0, 1, 1, 1),
(149, '1007', '102471594740', '15G0400001', 0, 1, 1, 1),
(150, '1007', '102471594740', '15G0300001', 0, 1, 1, 1),
(151, '1007', '101927814110', '15F2800006', 0, 0, 0, 0),
(152, '1007', '102471594740', '15D4500002', 0, 0, 0, 0),
(153, '1007', '102471594740', '1596500141', 0, 1, 1, 1),
(154, '1007', '101927814110', '1594400008', 0, 0, 0, 0),
(155, '1007', '102471591721', '04D8000001', 0, 1, 1, 1),
(156, '1007', '101927808400', '0454800060', 0, 1, 1, 1),
(157, '1007', '102471594740', '15G6000002', 0, 0, 0, 0),
(158, '1007', '102471591721', '0454800059', 0, 0, 0, 0),
(159, '1007', '102471591721', '0454800035', 0, 1, 1, 1),
(160, '1007', '102471591721', '0452100963', 0, 1, 1, 1),
(161, '1007', '101927798120', '0242301548', 0, 1, 1, 1),
(162, '1007', '102471574441', '0153600039', 0, 1, 1, 1),
(163, '1007', '102471574441', '0153600037', 0, 1, 1, 1),
(164, '1007', '101927748750', '0141500255', 0, 1, 1, 1),
(165, '1007', '101927748750', '0114400314', 0, 1, 1, 1),
(166, '1007', '102471574441', '0113800503', 0, 1, 1, 1),
(167, '1007', '102471591721', '0454800055', 1, 1, 0, 0),
(168, '1007', '102471591721', '16F1100006', 0, 1, 1, 1),
(169, '1008', '102471594740', '15G0400001', 0, 1, 1, 1),
(170, '1008', '101927808400', '0454800060', 0, 1, 1, 1),
(171, '1008', '102471591721', '0454800059', 0, 0, 0, 0),
(172, '1008', '102471591721', '0454800055', 0, 1, 1, 1),
(173, '1008', '102471591721', '0454800035', 0, 1, 1, 1),
(174, '1008', '102471591721', '0452100963', 0, 1, 1, 1),
(175, '1008', '101927798120', '0242301548', 0, 1, 1, 1),
(176, '1008', '102471574441', '0153600039', 0, 1, 1, 1),
(177, '1008', '102471574441', '0153600037', 0, 1, 1, 1),
(178, '1008', '102471591721', '04D8000001', 0, 1, 1, 1),
(179, '1008', '101927748750', '0141500255', 0, 1, 1, 1),
(180, '1008', '102471574441', '0113800503', 0, 1, 1, 1),
(181, '1008', '102471574441', '0113800502', 0, 1, 1, 1),
(182, '1008', '102471594740', '1596500141', 0, 1, 1, 1),
(183, '1008', '102471594740', '15D4500002', 0, 0, 0, 0),
(184, '1008', '101927814110', '15F2800006', 0, 0, 0, 0),
(185, '1008', '102471594740', '15G0300001', 0, 1, 1, 1),
(186, '1008', '102471591721', '16F1100006', 1, 1, 0, 0),
(187, '1008', '102471594740', '15G6000002', 0, 0, 0, 0),
(188, '1008', '101927748750', '0114400314', 0, 1, 1, 1),
(189, '1008', '101927814110', '1594400008', 0, 0, 0, 0),
(190, '1009', '102471574441', '0113800502', 0, 1, 1, 1),
(191, '1009', '102471591721', '0454800035', 0, 1, 1, 1),
(192, '1009', '102471594740', '15G0400001', 0, 1, 1, 1),
(193, '1009', '102471594740', '15G0300001', 1, 1, 0, 0),
(194, '1009', '101927814110', '15F2800006', 0, 0, 0, 0),
(195, '1009', '102471594740', '15D4500002', 0, 0, 0, 0),
(196, '1009', '102471594740', '1596500141', 0, 1, 1, 1),
(197, '1009', '101927814110', '1594400008', 0, 0, 0, 0),
(198, '1009', '102471591721', '04D8000001', 0, 1, 1, 1),
(199, '1009', '102471594740', '15G6000002', 0, 0, 0, 0),
(200, '1009', '101927808400', '0454800060', 0, 1, 1, 1),
(201, '1009', '102471591721', '0454800055', 0, 1, 1, 1),
(202, '1009', '102471591721', '0452100963', 0, 1, 1, 1),
(203, '1009', '101927798120', '0242301548', 0, 1, 1, 1),
(204, '1009', '102471574441', '0153600039', 0, 1, 1, 1),
(205, '1009', '102471574441', '0153600037', 0, 1, 1, 1),
(206, '1009', '101927748750', '0141500255', 0, 1, 1, 1),
(207, '1009', '101927748750', '0114400314', 0, 1, 1, 1),
(208, '1009', '102471574441', '0113800503', 0, 1, 1, 1),
(209, '1009', '102471591721', '0454800059', 0, 0, 0, 0),
(210, '1009', '102471591721', '16F1100006', 0, 1, 1, 1),
(211, '11032019', '102471574441', '0113800502', 0, 2, 2, 2),
(212, '11032019', '102471574441', '0113800503', 0, 2, 2, 2),
(213, '11032019', '101927748750', '0114400314', 0, 0, 0, 0),
(214, '11032019', '101927748750', '0141500255', 0, 0, 0, 0),
(215, '11032019', '102471574441', '0153600037', 0, 1, 1, 1),
(216, '11032019', '102471574441', '0153600039', 0, 1, 1, 1),
(217, '11032019', '101927798120', '0242301548', 0, 0, 0, 0),
(218, '11032019', '102471591721', '0452100963', 0, 1, 1, 1),
(219, '11032019', '102471591721', '0454800035', 0, 1, 1, 1),
(220, '11032019', '102471591721', '0454800055', 0, 1, 1, 1),
(221, '11032019', '102471591721', '0454800059', 0, 1, 1, 1),
(222, '11032019', '101927808400', '0454800060', 0, 0, 0, 0),
(223, '11032019', '102471591721', '04D8000001', 0, 1, 1, 1),
(224, '11032019', '101927814110', '1594400008', 0, 0, 0, 0),
(225, '11032019', '102471594740', '1596500141', 0, 1, 1, 1),
(226, '11032019', '102471594740', '15D4500002', 0, 1, 1, 1),
(227, '11032019', '101927814110', '15F2800006', 0, 0, 0, 0),
(228, '11032019', '102471594740', '15G0300001', 0, 1, 1, 1),
(229, '11032019', '102471594740', '15G0400001', 0, 1, 1, 1),
(230, '11032019', '102471594740', '15G6000002', 0, 1, 1, 1),
(231, '11032019', '102471591721', '16F1100006', 1, 1, 0, 0),
(232, '123', '102471574441', '0113800502', 1, 2, 1, 1),
(233, '123', '102471574441', '0113800503', 1, 2, 1, 1),
(234, '123', '101927748750', '0114400314', 0, 0, 0, 0),
(235, '123', '101927748750', '0141500255', 0, 0, 0, 0),
(236, '123', '102471574441', '0153600037', 1, 1, 0, 0),
(237, '123', '102471574441', '0153600039', 1, 1, 0, 0),
(238, '123', '101927798120', '0242301548', 0, 0, 0, 0),
(239, '123', '102471591721', '0452100963', 0, 1, 1, 1),
(240, '123', '102471591721', '0454800035', 0, 1, 1, 1),
(241, '123', '102471591721', '0454800055', 0, 1, 1, 1),
(242, '123', '102471591721', '0454800059', 0, 1, 1, 1),
(243, '123', '101927808400', '0454800060', 0, 0, 0, 0),
(244, '123', '102471591721', '04D8000001', 0, 1, 1, 1),
(245, '123', '101927814110', '1594400008', 0, 0, 0, 0),
(246, '123', '102471594740', '1596500141', 0, 1, 1, 1),
(247, '123', '102471594740', '15D4500002', 0, 1, 1, 1),
(248, '123', '101927814110', '15F2800006', 0, 0, 0, 0),
(249, '123', '102471594740', '15G0300001', 0, 1, 1, 1),
(250, '123', '102471594740', '15G0400001', 0, 1, 1, 1),
(251, '123', '102471594740', '15G6000002', 0, 1, 1, 1),
(252, '123', '102471591721', '16F1100006', 0, 1, 1, 1),
(253, '456', '102471574441', '0113800502', 0, 2, 2, 2),
(254, '456', '102471574441', '0113800503', 0, 2, 2, 2),
(255, '456', '102471574441', '0153600037', 0, 1, 1, 1),
(256, '456', '102471574441', '0153600039', 0, 1, 1, 1),
(257, '456', '102471591721', '0452100963', 1, 1, 0, 0),
(258, '456', '102471591721', '0454800035', 1, 1, 0, 0),
(259, '456', '102471591721', '0454800055', 0, 1, 1, 1),
(260, '456', '102471591721', '0454800059', 0, 1, 1, 1),
(261, '456', '102471591721', '04D8000001', 0, 1, 1, 1),
(262, '456', '102471594740', '1596500141', 0, 1, 1, 1),
(263, '456', '102471594740', '15D4500002', 0, 1, 1, 1),
(264, '456', '102471594740', '15G0300001', 0, 1, 1, 1),
(265, '456', '102471594740', '15G0400001', 0, 1, 1, 1),
(266, '456', '102471594740', '15G6000002', 0, 1, 1, 1),
(267, '456', '102471591721', '16F1100006', 0, 1, 1, 1),
(268, '789', '102471574441', '0113800502', 0, 2, 2, 2),
(269, '789', '102471574441', '0113800503', 0, 2, 2, 2),
(270, '789', '102471574441', '0153600037', 0, 1, 1, 1),
(271, '789', '102471574441', '0153600039', 0, 1, 1, 1),
(272, '789', '102471591721', '0452100963', 0, 1, 1, 1),
(273, '789', '102471591721', '0454800035', 0, 1, 1, 1),
(274, '789', '102471591721', '0454800055', 1, 1, 0, 0),
(275, '789', '102471591721', '0454800059', 0, 1, 1, 1),
(276, '789', '102471591721', '04D8000001', 1, 1, 0, 0),
(277, '789', '102471594740', '1596500141', 0, 1, 1, 1),
(278, '789', '102471594740', '15D4500002', 0, 1, 1, 1),
(279, '789', '102471594740', '15G0300001', 0, 1, 1, 1),
(280, '789', '102471594740', '15G0400001', 0, 1, 1, 1),
(281, '789', '102471594740', '15G6000002', 0, 1, 1, 1),
(282, '789', '102471591721', '16F1100006', 0, 1, 1, 1),
(283, '159', '102471574441', '0113800502', 0, 2, 2, 2),
(284, '159', '102471574441', '0113800503', 0, 2, 2, 2),
(285, '159', '102471574441', '0153600037', 0, 1, 1, 1),
(286, '159', '102471574441', '0153600039', 0, 1, 1, 1),
(287, '159', '102471591721', '0452100963', 0, 1, 1, 1),
(288, '159', '102471591721', '0454800035', 0, 1, 1, 1),
(289, '159', '102471591721', '0454800055', 0, 1, 1, 1),
(290, '159', '102471591721', '0454800059', 0, 1, 1, 1),
(291, '159', '102471591721', '04D8000001', 1, 1, 0, 0),
(292, '159', '102471594740', '1596500141', 0, 1, 1, 1),
(293, '159', '102471594740', '15D4500002', 0, 1, 1, 1),
(294, '159', '102471594740', '15G0300001', 0, 1, 1, 1),
(295, '159', '102471594740', '15G0400001', 0, 1, 1, 1),
(296, '159', '102471594740', '15G6000002', 0, 1, 1, 1),
(297, '159', '102471591721', '16F1100006', 1, 1, 0, 0),
(298, '357', '102471594740', '15G0400001', 0, 1, 1, 1),
(299, '357', '102471594740', '15G0300001', 0, 1, 1, 1),
(300, '357', '102471594740', '15D4500002', 0, 1, 1, 1),
(301, '357', '102471594740', '1596500141', 0, 1, 1, 1),
(302, '357', '102471591721', '04D8000001', 0, 1, 1, 1),
(303, '357', '102471591721', '0454800059', 0, 1, 1, 1),
(304, '357', '102471591721', '0454800055', 0, 1, 1, 1),
(305, '357', '102471591721', '0454800035', 0, 1, 1, 1),
(306, '357', '102471591721', '0452100963', 0, 1, 1, 1),
(307, '357', '102471574441', '0153600039', 0, 1, 1, 1),
(308, '357', '102471574441', '0153600037', 0, 1, 1, 1),
(309, '357', '102471574441', '0113800503', 0, 2, 2, 2),
(310, '357', '102471574441', '0113800502', 0, 2, 2, 2),
(311, '357', '102471594740', '15G6000002', 1, 1, 0, 0),
(312, '357', '102471591721', '16F1100006', 0, 1, 1, 1),
(313, '258', '102471574441', '0113800502', 0, 2, 2, 2),
(314, '258', '102471591721', '16F1100006', 0, 0, 0, 0),
(315, '258', '102471594740', '15G6000002', 0, 1, 1, 1),
(316, '258', '102471594740', '15G0400001', 0, 1, 1, 1),
(317, '258', '102471594740', '15G0300001', 1, 1, 0, 0),
(318, '258', '102471594740', '15D4500002', 0, 1, 1, 1),
(319, '258', '102471594740', '1596500141', 0, 1, 1, 1),
(320, '258', '102471591721', '04D8000001', 0, 1, 1, 1),
(321, '258', '102471591721', '0454800059', 0, 0, 0, 0),
(322, '258', '102471591721', '0454800055', 0, 1, 1, 1),
(323, '258', '102471591721', '0454800035', 0, 1, 1, 1),
(324, '258', '102471591721', '0452100963', 0, 1, 1, 1),
(325, '258', '102471574441', '0153600039', 0, 1, 1, 1),
(326, '258', '102471574441', '0153600037', 0, 1, 1, 1),
(327, '258', '102471574441', '0113800503', 0, 2, 2, 2),
(328, '147', '102471574441', '0113800502', 0, 2, 2, 2),
(329, '147', '102471591721', '16F1100006', 0, 0, 0, 0),
(330, '147', '102471594740', '15G6000002', 0, 0, 0, 0),
(331, '147', '102471594740', '15G0400001', 0, 1, 1, 1),
(332, '147', '102471594740', '15G0300001', 0, 1, 1, 1),
(333, '147', '102471594740', '15D4500002', 0, 0, 0, 0),
(334, '147', '102471594740', '1596500141', 0, 1, 1, 1),
(335, '147', '102471591721', '04D8000001', 0, 1, 1, 1),
(336, '147', '102471591721', '0454800059', 0, 0, 0, 0),
(337, '147', '102471591721', '0454800055', 0, 1, 1, 1),
(338, '147', '102471591721', '0454800035', 0, 1, 1, 1),
(339, '147', '102471591721', '0452100963', 0, 1, 1, 1),
(340, '147', '102471574441', '0153600039', 0, 1, 1, 1),
(341, '147', '102471574441', '0153600037', 0, 1, 1, 1),
(342, '147', '102471574441', '0113800503', 0, 2, 2, 2),
(343, '9988', '102471594740', '15G0400001', 0, 1, 1, 1),
(344, '9988', '102471594740', '15G0300001', 0, 1, 1, 1),
(345, '9988', '102471594740', '15D4500002', 0, 0, 0, 0),
(346, '9988', '102471594740', '1596500141', 1, 1, 0, 0),
(347, '9988', '102471591721', '04D8000001', 0, 1, 1, 1),
(348, '9988', '102471591721', '0454800059', 0, 0, 0, 0),
(349, '9988', '102471591721', '0454800055', 0, 1, 1, 1),
(350, '9988', '102471591721', '0454800035', 0, 1, 1, 1),
(351, '9988', '102471591721', '0452100963', 0, 1, 1, 1),
(352, '9988', '102471574441', '0153600039', 0, 1, 1, 1),
(353, '9988', '102471574441', '0153600037', 0, 1, 1, 1),
(354, '9988', '102471574441', '0113800503', 0, 2, 2, 2),
(355, '9988', '102471574441', '0113800502', 0, 2, 2, 2),
(356, '9988', '102471594740', '15G6000002', 0, 0, 0, 0),
(357, '9988', '102471591721', '16F1100006', 0, 0, 0, 0),
(358, '369', '102471574441', '0113800502', 0, 2, 2, 2),
(359, '369', '102471591721', '16F1100006', 0, 0, 0, 0),
(360, '369', '102471594740', '15G6000002', 0, 0, 0, 0),
(361, '369', '102471594740', '15G0400001', 1, 1, 0, 0),
(362, '369', '102471594740', '15G0300001', 0, 1, 1, 1),
(363, '369', '102471594740', '15D4500002', 0, 0, 0, 0),
(364, '369', '102471594740', '1596500141', 0, 1, 1, 1),
(365, '369', '102471591721', '04D8000001', 0, 1, 1, 1),
(366, '369', '102471591721', '0454800059', 0, 0, 0, 0),
(367, '369', '102471591721', '0454800055', 0, 1, 1, 1),
(368, '369', '102471591721', '0454800035', 0, 1, 1, 1),
(369, '369', '102471591721', '0452100963', 0, 1, 1, 1),
(370, '369', '102471574441', '0153600039', 0, 1, 1, 1),
(371, '369', '102471574441', '0153600037', 0, 1, 1, 1),
(372, '369', '102471574441', '0113800503', 0, 2, 2, 2),
(373, '708', '102471594740', '15G0400001', 0, 1, 1, 1),
(374, '708', '102471594740', '15G0300001', 0, 1, 1, 1),
(375, '708', '102471594740', '15D4500002', 0, 0, 0, 0),
(376, '708', '102471594740', '1596500141', 0, 1, 1, 1),
(377, '708', '101752973820', '04D8000001', 0, 0, 0, 0),
(378, '708', '102471591721', '0454800059', 0, 0, 0, 0),
(379, '708', '102471591721', '0454800055', 0, 1, 1, 1),
(380, '708', '102471591721', '0454800035', 0, 1, 1, 1),
(381, '708', '102471591721', '0452100963', 0, 0, 0, 0),
(382, '708', '102471574441', '0153600039', 0, 1, 1, 1),
(383, '708', '102471574441', '0153600037', 0, 1, 1, 1),
(384, '708', '102471574441', '0113800503', 0, 2, 2, 2),
(385, '708', '102471574441', '0113800502', 0, 2, 2, 2),
(386, '708', '102471594740', '15G6000002', 0, 0, 0, 0),
(387, '708', '102471591721', '16F1100006', 0, 0, 0, 0),
(388, '123abc', '102471574441', '0113800502', 0, 2, 2, 2),
(389, '123abc', '102471591721', '16F1100006', 0, 1, 1, 1),
(390, '123abc', '102471594740', '15G6000002', 0, 1, 1, 1),
(391, '123abc', '102471594740', '15G0400001', 0, 1, 1, 1),
(392, '123abc', '102471594740', '15G0300001', 0, 1, 1, 1),
(393, '123abc', '102471594740', '15D4500002', 0, 1, 1, 1),
(394, '123abc', '102471594740', '1596500141', 0, 1, 1, 1),
(395, '123abc', '102471591721', '04D8000001', 0, 1, 1, 1),
(396, '123abc', '102471591721', '0454800059', 0, 1, 1, 1),
(397, '123abc', '102471591721', '0454800055', 0, 1, 1, 1),
(398, '123abc', '102471591721', '0454800035', 0, 1, 1, 1),
(399, '123abc', '102471591721', '0452100963', 0, 1, 1, 1),
(400, '123abc', '102471574441', '0153600039', 0, 1, 1, 1),
(401, '123abc', '102471574441', '0153600037', 0, 1, 1, 1),
(402, '123abc', '102471574441', '0113800503', 0, 2, 2, 2),
(403, '456def', '102471594740', '15G0400001', 0, 1, 1, 1),
(404, '456def', '102471594740', '15G0300001', 0, 1, 1, 1),
(405, '456def', '102471594740', '15D4500002', 0, 1, 1, 1),
(406, '456def', '102471594740', '1596500141', 0, 1, 1, 1),
(407, '456def', '102471591721', '04D8000001', 0, 1, 1, 1),
(408, '456def', '102471591721', '0454800059', 0, 1, 1, 1),
(409, '456def', '102471591721', '0454800055', 0, 1, 1, 1),
(410, '456def', '102471591721', '0454800035', 0, 1, 1, 1),
(411, '456def', '102471591721', '0452100963', 0, 1, 1, 1),
(412, '456def', '102471574441', '0153600039', 0, 1, 1, 1),
(413, '456def', '102471574441', '0153600037', 0, 1, 1, 1),
(414, '456def', '102471574441', '0113800503', 0, 2, 2, 2),
(415, '456def', '102471574441', '0113800502', 0, 2, 2, 2),
(416, '456def', '102471594740', '15G6000002', 0, 1, 1, 1),
(417, '456def', '102471591721', '16F1100006', 0, 1, 1, 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_seal_product`
--

CREATE TABLE `wh_seal_product` (
  `id` bigint(20) NOT NULL,
  `seal_number` varchar(255) NOT NULL,
  `product_code` varchar(255) NOT NULL,
  `quantity_export` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Đang đổ dữ liệu cho bảng `wh_seal_product`
--

INSERT INTO `wh_seal_product` (`id`, `seal_number`, `product_code`, `quantity_export`, `created_at`) VALUES
(1, '1000', '0113800502', 2, '2020-11-03 01:23:34'),
(2, '1006', '0452100963', 1, '2020-11-03 01:23:34'),
(3, '1006', '0454800035', 1, '2020-11-03 01:23:34'),
(4, '1006', '0454800055', 1, '2020-11-03 01:23:34'),
(5, '1006', '0454800059', 1, '2020-11-03 01:23:34'),
(6, '1006', '0454800060', 1, '2020-11-03 01:23:34'),
(7, '1006', '04D8000001', 1, '2020-11-03 01:23:34'),
(8, '1006', '1594400008', 1, '2020-11-03 01:23:34'),
(9, '1006', '1596500141', 1, '2020-11-03 01:23:34'),
(10, '1006', '15D4500002', 1, '2020-11-03 01:23:34'),
(11, '1006', '15F2800006', 1, '2020-11-03 01:23:34'),
(12, '1006', '0242301548', 1, '2020-11-03 01:23:34'),
(13, '1006', '15G0300001', 1, '2020-11-03 01:23:34'),
(14, '1006', '15G6000002', 1, '2020-11-03 01:23:34'),
(15, '1006', '16F1100006', 1, '2020-11-03 01:23:34'),
(16, '1007', '0113800502', 1, '2020-11-03 01:23:34'),
(17, '1007', '0113800503', 1, '2020-11-03 01:23:34'),
(18, '1007', '0114400314', 1, '2020-11-03 01:23:34'),
(19, '1007', '0141500255', 1, '2020-11-03 01:23:34'),
(20, '1007', '0153600037', 1, '2020-11-03 01:23:34'),
(21, '1007', '0153600039', 1, '2020-11-03 01:23:34'),
(22, '1007', '0242301548', 1, '2020-11-03 01:23:34'),
(23, '1007', '0452100963', 1, '2020-11-03 01:23:34'),
(24, '1006', '15G0400001', 1, '2020-11-03 01:23:34'),
(25, '1007', '0454800035', 1, '2020-11-03 01:23:34'),
(26, '1006', '0153600039', 1, '2020-11-03 01:23:34'),
(27, '1006', '0141500255', 1, '2020-11-03 01:23:34'),
(28, '1005', '0114400314', 1, '2020-11-03 01:23:34'),
(29, '1005', '0141500255', 1, '2020-11-03 01:23:34'),
(30, '1005', '0153600037', 1, '2020-11-03 01:23:34'),
(31, '1005', '0153600039', 1, '2020-11-03 01:23:34'),
(32, '1005', '0242301548', 1, '2020-11-03 01:23:34'),
(33, '1005', '0452100963', 1, '2020-11-03 01:23:34'),
(34, '1005', '0454800035', 1, '2020-11-03 01:23:34'),
(35, '1005', '0454800055', 1, '2020-11-03 01:23:34'),
(36, '1005', '0454800059', 1, '2020-11-03 01:23:34'),
(37, '1005', '0454800060', 1, '2020-11-03 01:23:34'),
(38, '1006', '0153600037', 1, '2020-11-03 01:23:34'),
(39, '1005', '04D8000001', 1, '2020-11-03 01:23:34'),
(40, '1005', '1596500141', 1, '2020-11-03 01:23:34'),
(41, '1005', '15D4500002', 1, '2020-11-03 01:23:34'),
(42, '1005', '15F2800006', 1, '2020-11-03 01:23:34'),
(43, '1005', '15G0300001', 1, '2020-11-03 01:23:34'),
(44, '1005', '15G0400001', 1, '2020-11-03 01:23:34'),
(45, '1005', '15G6000002', 1, '2020-11-03 01:23:34'),
(46, '1005', '16F1100006', 1, '2020-11-03 01:23:34'),
(47, '1006', '0113800502', 1, '2020-11-03 01:23:34'),
(48, '1006', '0113800503', 1, '2020-11-03 01:23:34'),
(49, '1006', '0114400314', 1, '2020-11-03 01:23:34'),
(50, '1005', '1594400008', 1, '2020-11-03 01:23:34'),
(51, '1007', '0454800055', 1, '2020-11-03 01:23:34'),
(52, '1007', '0454800059', 1, '2020-11-03 01:23:34'),
(53, '1007', '0454800060', 1, '2020-11-03 01:23:34'),
(54, '1008', '15G0400001', 1, '2020-11-03 01:23:34'),
(55, '1008', '15G6000002', 1, '2020-11-03 01:23:34'),
(56, '1008', '16F1100006', 1, '2020-11-03 01:23:34'),
(57, '1009', '0113800502', 1, '2020-11-03 01:23:34'),
(58, '1009', '0113800503', 1, '2020-11-03 01:23:34'),
(59, '1009', '0114400314', 1, '2020-11-03 01:23:34'),
(60, '1009', '0141500255', 1, '2020-11-03 01:23:34'),
(61, '1009', '0153600037', 1, '2020-11-03 01:23:34'),
(62, '1009', '0153600039', 1, '2020-11-03 01:23:34'),
(63, '1009', '0242301548', 1, '2020-11-03 01:23:34'),
(64, '1008', '15G0300001', 1, '2020-11-03 01:23:34'),
(65, '1009', '0452100963', 1, '2020-11-03 01:23:34'),
(66, '1009', '0454800055', 1, '2020-11-03 01:23:34'),
(67, '1009', '0454800059', 1, '2020-11-03 01:23:34'),
(68, '1009', '0454800060', 1, '2020-11-03 01:23:34'),
(69, '1009', '04D8000001', 1, '2020-11-03 01:23:34'),
(70, '1009', '1594400008', 1, '2020-11-03 01:23:34'),
(71, '1009', '1596500141', 1, '2020-11-03 01:23:34'),
(72, '1009', '15D4500002', 1, '2020-11-03 01:23:34'),
(73, '1009', '15F2800006', 1, '2020-11-03 01:23:34'),
(74, '1009', '15G0300001', 1, '2020-11-03 01:23:34'),
(75, '1009', '15G0400001', 1, '2020-11-03 01:23:34'),
(76, '1009', '0454800035', 1, '2020-11-03 01:23:34'),
(77, '1008', '15F2800006', 1, '2020-11-03 01:23:34'),
(78, '1008', '15D4500002', 1, '2020-11-03 01:23:34'),
(79, '1008', '1596500141', 1, '2020-11-03 01:23:34'),
(80, '1007', '04D8000001', 1, '2020-11-03 01:23:34'),
(81, '1007', '1594400008', 1, '2020-11-03 01:23:34'),
(82, '1007', '1596500141', 1, '2020-11-03 01:23:34'),
(83, '1007', '15D4500002', 1, '2020-11-03 01:23:34'),
(84, '1007', '15F2800006', 1, '2020-11-03 01:23:34'),
(85, '1007', '15G0300001', 1, '2020-11-03 01:23:34'),
(86, '1007', '15G0400001', 1, '2020-11-03 01:23:34'),
(87, '1007', '15G6000002', 1, '2020-11-03 01:23:34'),
(88, '1007', '16F1100006', 1, '2020-11-03 01:23:34'),
(89, '1008', '0113800502', 1, '2020-11-03 01:23:34'),
(90, '1008', '0113800503', 1, '2020-11-03 01:23:34'),
(91, '1008', '0114400314', 1, '2020-11-03 01:23:34'),
(92, '1008', '0141500255', 1, '2020-11-03 01:23:34'),
(93, '1008', '0153600037', 1, '2020-11-03 01:23:34'),
(94, '1008', '0153600039', 1, '2020-11-03 01:23:34'),
(95, '1008', '0242301548', 1, '2020-11-03 01:23:34'),
(96, '1008', '0452100963', 1, '2020-11-03 01:23:34'),
(97, '1008', '0454800035', 1, '2020-11-03 01:23:34'),
(98, '1008', '0454800055', 1, '2020-11-03 01:23:34'),
(99, '1008', '0454800059', 1, '2020-11-03 01:23:34'),
(100, '1008', '0454800060', 1, '2020-11-03 01:23:34'),
(101, '1008', '04D8000001', 1, '2020-11-03 01:23:34'),
(102, '1008', '1594400008', 1, '2020-11-03 01:23:34'),
(103, '1005', '0113800503', 1, '2020-11-03 01:23:34'),
(104, '1005', '0113800502', 1, '2020-11-03 01:23:34'),
(105, '1004', '16F1100006', 1, '2020-11-03 01:23:34'),
(106, '1004', '15G6000002', 1, '2020-11-03 01:23:34'),
(107, '1001', '0452100963', 1, '2020-11-03 01:23:34'),
(108, '1001', '0454800035', 1, '2020-11-03 01:23:34'),
(109, '1001', '0454800055', 1, '2020-11-03 01:23:34'),
(110, '1001', '0454800059', 1, '2020-11-03 01:23:34'),
(111, '1001', '0454800060', 1, '2020-11-03 01:23:34'),
(112, '1001', '04D8000001', 1, '2020-11-03 01:23:34'),
(113, '1001', '1594400008', 1, '2020-11-03 01:23:34'),
(114, '1001', '1596500141', 1, '2020-11-03 01:23:34'),
(115, '1001', '15D4500002', 1, '2020-11-03 01:23:34'),
(116, '1001', '15F2800006', 1, '2020-11-03 01:23:34'),
(117, '1001', '0242301548', 1, '2020-11-03 01:23:34'),
(118, '1001', '15G0300001', 1, '2020-11-03 01:23:34'),
(119, '1001', '15G6000002', 1, '2020-11-03 01:23:34'),
(120, '1001', '16F1100006', 1, '2020-11-03 01:23:34'),
(121, '1002', '0113800502', 1, '2020-11-03 01:23:34'),
(122, '1002', '0113800503', 1, '2020-11-03 01:23:34'),
(123, '1002', '0114400314', 1, '2020-11-03 01:23:34'),
(124, '1002', '0141500255', 1, '2020-11-03 01:23:34'),
(125, '1002', '0153600037', 1, '2020-11-03 01:23:34'),
(126, '1002', '0153600039', 1, '2020-11-03 01:23:34'),
(127, '1002', '0242301548', 1, '2020-11-03 01:23:34'),
(128, '1002', '0452100963', 1, '2020-11-03 01:23:34'),
(129, '1001', '15G0400001', 1, '2020-11-03 01:23:34'),
(130, '1001', '0153600039', 1, '2020-11-03 01:23:34'),
(131, '1001', '0153600037', 1, '2020-11-03 01:23:34'),
(132, '1001', '0141500255', 2, '2020-11-03 01:23:34'),
(133, '1000', '0113800503', 2, '2020-11-03 01:23:34'),
(134, '1000', '0114400314', 1, '2020-11-03 01:23:34'),
(135, '1000', '0141500255', 2, '2020-11-03 01:23:34'),
(136, '1000', '0153600037', 1, '2020-11-03 01:23:34'),
(137, '1000', '0153600039', 1, '2020-11-03 01:23:34'),
(138, '1000', '0242301548', 1, '2020-11-03 01:23:34'),
(139, '1000', '0452100963', 1, '2020-11-03 01:23:34'),
(140, '1000', '0454800035', 1, '2020-11-03 01:23:34'),
(141, '1000', '0454800055', 1, '2020-11-03 01:23:34'),
(142, '1000', '0454800059', 1, '2020-11-03 01:23:34'),
(143, '1000', '0454800060', 1, '2020-11-03 01:23:34'),
(144, '1000', '04D8000001', 1, '2020-11-03 01:23:34'),
(145, '1000', '1594400008', 1, '2020-11-03 01:23:34'),
(146, '1000', '1596500141', 1, '2020-11-03 01:23:34'),
(147, '1000', '15D4500002', 1, '2020-11-03 01:23:34'),
(148, '1000', '15F2800006', 1, '2020-11-03 01:23:34'),
(149, '1000', '15G0300001', 1, '2020-11-03 01:23:34'),
(150, '1000', '15G0400001', 1, '2020-11-03 01:23:34'),
(151, '1000', '15G6000002', 1, '2020-11-03 01:23:34'),
(152, '1000', '16F1100006', 1, '2020-11-03 01:23:34'),
(153, '1001', '0113800502', 2, '2020-11-03 01:23:34'),
(154, '1001', '0113800503', 2, '2020-11-03 01:23:34'),
(155, '1001', '0114400314', 1, '2020-11-03 01:23:34'),
(156, '1002', '0454800035', 1, '2020-11-03 01:23:34'),
(157, '1009', '15G6000002', 1, '2020-11-03 01:23:34'),
(158, '1002', '0454800055', 1, '2020-11-03 01:23:34'),
(159, '1002', '0454800060', 1, '2020-11-03 01:23:34'),
(160, '1003', '15G0400001', 1, '2020-11-03 01:23:34'),
(161, '1003', '15G6000002', 1, '2020-11-03 01:23:34'),
(162, '1003', '16F1100006', 1, '2020-11-03 01:23:34'),
(163, '1004', '0113800502', 1, '2020-11-03 01:23:34'),
(164, '1004', '0113800503', 1, '2020-11-03 01:23:34'),
(165, '1004', '0114400314', 1, '2020-11-03 01:23:34'),
(166, '1004', '0141500255', 1, '2020-11-03 01:23:34'),
(167, '1004', '0153600037', 1, '2020-11-03 01:23:34'),
(168, '1004', '0153600039', 1, '2020-11-03 01:23:34'),
(169, '1004', '0242301548', 1, '2020-11-03 01:23:34'),
(170, '1003', '15G0300001', 1, '2020-11-03 01:23:34'),
(171, '1004', '0452100963', 1, '2020-11-03 01:23:34'),
(172, '1004', '0454800055', 1, '2020-11-03 01:23:34'),
(173, '1004', '0454800059', 1, '2020-11-03 01:23:34'),
(174, '1004', '0454800060', 1, '2020-11-03 01:23:34'),
(175, '1004', '04D8000001', 1, '2020-11-03 01:23:34'),
(176, '1004', '1594400008', 1, '2020-11-03 01:23:34'),
(177, '1004', '1596500141', 1, '2020-11-03 01:23:34'),
(178, '1004', '15D4500002', 1, '2020-11-03 01:23:34'),
(179, '1004', '15F2800006', 1, '2020-11-03 01:23:34'),
(180, '1004', '15G0300001', 1, '2020-11-03 01:23:34'),
(181, '1004', '15G0400001', 1, '2020-11-03 01:23:34'),
(182, '1004', '0454800035', 1, '2020-11-03 01:23:34'),
(183, '1003', '15F2800006', 1, '2020-11-03 01:23:34'),
(184, '1003', '15D4500002', 1, '2020-11-03 01:23:34'),
(185, '1003', '1596500141', 1, '2020-11-03 01:23:34'),
(186, '1002', '04D8000001', 1, '2020-11-03 01:23:34'),
(187, '1002', '1594400008', 1, '2020-11-03 01:23:34'),
(188, '1002', '1596500141', 1, '2020-11-03 01:23:34'),
(189, '1002', '15D4500002', 1, '2020-11-03 01:23:34'),
(190, '1002', '15F2800006', 1, '2020-11-03 01:23:34'),
(191, '1002', '15G0300001', 1, '2020-11-03 01:23:34'),
(192, '1002', '15G0400001', 1, '2020-11-03 01:23:34'),
(193, '1002', '15G6000002', 1, '2020-11-03 01:23:34'),
(194, '1002', '16F1100006', 1, '2020-11-03 01:23:34'),
(195, '1003', '0113800502', 2, '2020-11-03 01:23:34'),
(196, '1003', '0113800503', 2, '2020-11-03 01:23:34'),
(197, '1003', '0114400314', 1, '2020-11-03 01:23:34'),
(198, '1003', '0141500255', 2, '2020-11-03 01:23:34'),
(199, '1003', '0153600037', 1, '2020-11-03 01:23:34'),
(200, '1003', '0153600039', 1, '2020-11-03 01:23:34'),
(201, '1003', '0242301548', 1, '2020-11-03 01:23:34'),
(202, '1003', '0452100963', 1, '2020-11-03 01:23:34'),
(203, '1003', '0454800035', 1, '2020-11-03 01:23:34'),
(204, '1003', '0454800055', 1, '2020-11-03 01:23:34'),
(205, '1003', '0454800059', 1, '2020-11-03 01:23:34'),
(206, '1003', '0454800060', 1, '2020-11-03 01:23:34'),
(207, '1003', '04D8000001', 1, '2020-11-03 01:23:34'),
(208, '1003', '1594400008', 1, '2020-11-03 01:23:34'),
(209, '1002', '0454800059', 1, '2020-11-03 01:23:34'),
(210, '1009', '16F1100006', 1, '2020-11-03 01:23:34'),
(232, '11032019', '0113800502', 2, '2020-11-03 07:00:02'),
(233, '11032019', '15G0400001', 1, '2020-11-03 07:00:02'),
(234, '11032019', '15G0300001', 1, '2020-11-03 07:00:02'),
(235, '11032019', '15F2800006', 0, '2020-11-03 07:00:02'),
(236, '11032019', '15D4500002', 1, '2020-11-03 07:00:02'),
(237, '11032019', '1596500141', 1, '2020-11-03 07:00:02'),
(238, '11032019', '1594400008', 0, '2020-11-03 07:00:02'),
(239, '11032019', '04D8000001', 1, '2020-11-03 07:00:02'),
(240, '11032019', '0454800060', 0, '2020-11-03 07:00:02'),
(241, '11032019', '15G6000002', 1, '2020-11-03 07:00:02'),
(242, '11032019', '0454800059', 1, '2020-11-03 07:00:02'),
(243, '11032019', '0454800035', 1, '2020-11-03 07:00:02'),
(244, '11032019', '0452100963', 1, '2020-11-03 07:00:02'),
(245, '11032019', '0242301548', 0, '2020-11-03 07:00:02'),
(246, '11032019', '0153600039', 1, '2020-11-03 07:00:02'),
(247, '11032019', '0153600037', 1, '2020-11-03 07:00:02'),
(248, '11032019', '0141500255', 0, '2020-11-03 07:00:02'),
(249, '11032019', '0114400314', 0, '2020-11-03 07:00:02'),
(250, '11032019', '0113800503', 2, '2020-11-03 07:00:02'),
(251, '11032019', '0454800055', 1, '2020-11-03 07:00:02'),
(252, '11032019', '16F1100006', 1, '2020-11-03 07:00:02'),
(253, '123', '0113800502', 2, '2020-11-06 08:18:38'),
(254, '123', '15G0400001', 1, '2020-11-06 08:18:38'),
(255, '123', '15G0300001', 1, '2020-11-06 08:18:38'),
(256, '123', '15F2800006', 0, '2020-11-06 08:18:38'),
(257, '123', '15D4500002', 1, '2020-11-06 08:18:38'),
(258, '123', '1596500141', 1, '2020-11-06 08:18:38'),
(259, '123', '1594400008', 0, '2020-11-06 08:18:38'),
(260, '123', '04D8000001', 1, '2020-11-06 08:18:38'),
(261, '123', '0454800060', 0, '2020-11-06 08:18:38'),
(262, '123', '15G6000002', 1, '2020-11-06 08:18:38'),
(263, '123', '0454800059', 1, '2020-11-06 08:18:38'),
(264, '123', '0454800035', 1, '2020-11-06 08:18:38'),
(265, '123', '0452100963', 1, '2020-11-06 08:18:38'),
(266, '123', '0242301548', 0, '2020-11-06 08:18:38'),
(267, '123', '0153600039', 1, '2020-11-06 08:18:38'),
(268, '123', '0153600037', 1, '2020-11-06 08:18:38'),
(269, '123', '0141500255', 0, '2020-11-06 08:18:38'),
(270, '123', '0114400314', 0, '2020-11-06 08:18:38'),
(271, '123', '0113800503', 2, '2020-11-06 08:18:38'),
(272, '123', '0454800055', 1, '2020-11-06 08:18:38'),
(273, '123', '16F1100006', 1, '2020-11-06 08:18:38'),
(274, '456', '0113800502', 2, '2020-11-06 08:35:53'),
(275, '456', '0113800503', 2, '2020-11-06 08:35:53'),
(276, '456', '0153600037', 1, '2020-11-06 08:35:53'),
(277, '456', '0153600039', 1, '2020-11-06 08:35:53'),
(278, '456', '0452100963', 1, '2020-11-06 08:35:53'),
(279, '456', '0454800035', 1, '2020-11-06 08:35:53'),
(280, '456', '0454800055', 1, '2020-11-06 08:35:53'),
(281, '456', '0454800059', 1, '2020-11-06 08:35:53'),
(282, '456', '04D8000001', 1, '2020-11-06 08:35:53'),
(283, '456', '1596500141', 1, '2020-11-06 08:35:53'),
(284, '456', '15D4500002', 1, '2020-11-06 08:35:53'),
(285, '456', '15G0300001', 1, '2020-11-06 08:35:53'),
(286, '456', '15G0400001', 1, '2020-11-06 08:35:53'),
(287, '456', '15G6000002', 1, '2020-11-06 08:35:53'),
(288, '456', '16F1100006', 1, '2020-11-06 08:35:53'),
(289, '789', '0113800502', 2, '2020-11-06 08:37:01'),
(290, '789', '0113800503', 2, '2020-11-06 08:37:01'),
(291, '789', '0153600037', 1, '2020-11-06 08:37:01'),
(292, '789', '0153600039', 1, '2020-11-06 08:37:01'),
(293, '789', '0452100963', 1, '2020-11-06 08:37:01'),
(294, '789', '0454800035', 1, '2020-11-06 08:37:01'),
(295, '789', '0454800055', 1, '2020-11-06 08:37:01'),
(296, '789', '0454800059', 1, '2020-11-06 08:37:01'),
(297, '789', '04D8000001', 1, '2020-11-06 08:37:01'),
(298, '789', '1596500141', 1, '2020-11-06 08:37:01'),
(299, '789', '15D4500002', 1, '2020-11-06 08:37:01'),
(300, '789', '15G0300001', 1, '2020-11-06 08:37:01'),
(301, '789', '15G0400001', 1, '2020-11-06 08:37:01'),
(302, '789', '15G6000002', 1, '2020-11-06 08:37:01'),
(303, '789', '16F1100006', 1, '2020-11-06 08:37:01'),
(304, '159', '0113800502', 2, '2020-11-06 08:37:38'),
(305, '159', '0113800503', 2, '2020-11-06 08:37:38'),
(306, '159', '0153600037', 1, '2020-11-06 08:37:38'),
(307, '159', '0153600039', 1, '2020-11-06 08:37:38'),
(308, '159', '0452100963', 1, '2020-11-06 08:37:38'),
(309, '159', '0454800035', 1, '2020-11-06 08:37:38'),
(310, '159', '0454800055', 1, '2020-11-06 08:37:38'),
(311, '159', '0454800059', 1, '2020-11-06 08:37:39'),
(312, '159', '04D8000001', 1, '2020-11-06 08:37:39'),
(313, '159', '1596500141', 1, '2020-11-06 08:37:39'),
(314, '159', '15D4500002', 1, '2020-11-06 08:37:39'),
(315, '159', '15G0300001', 1, '2020-11-06 08:37:39'),
(316, '159', '15G0400001', 1, '2020-11-06 08:37:39'),
(317, '159', '15G6000002', 1, '2020-11-06 08:37:39'),
(318, '159', '16F1100006', 1, '2020-11-06 08:37:39'),
(319, '258', '0113800502', 2, '2020-11-06 08:45:02'),
(320, '357', '15G0400001', 1, '2020-11-06 08:45:02'),
(321, '357', '15G0300001', 1, '2020-11-06 08:45:02'),
(322, '357', '15D4500002', 1, '2020-11-06 08:45:02'),
(323, '357', '1596500141', 1, '2020-11-06 08:45:02'),
(324, '357', '04D8000001', 1, '2020-11-06 08:45:02'),
(325, '357', '0454800059', 1, '2020-11-06 08:45:02'),
(326, '357', '0454800055', 1, '2020-11-06 08:45:02'),
(327, '357', '0454800035', 1, '2020-11-06 08:45:02'),
(328, '357', '0452100963', 1, '2020-11-06 08:45:02'),
(329, '357', '0153600039', 1, '2020-11-06 08:45:02'),
(330, '357', '0153600037', 1, '2020-11-06 08:45:02'),
(331, '357', '0113800503', 2, '2020-11-06 08:45:02'),
(332, '357', '0113800502', 2, '2020-11-06 08:45:02'),
(333, '258', '16F1100006', 1, '2020-11-06 08:45:02'),
(334, '258', '15G6000002', 1, '2020-11-06 08:45:02'),
(335, '258', '15G0400001', 1, '2020-11-06 08:45:02'),
(336, '258', '15G0300001', 1, '2020-11-06 08:45:02'),
(337, '258', '15D4500002', 1, '2020-11-06 08:45:02'),
(338, '258', '1596500141', 1, '2020-11-06 08:45:02'),
(339, '258', '04D8000001', 1, '2020-11-06 08:45:02'),
(340, '258', '0454800059', 1, '2020-11-06 08:45:02'),
(341, '258', '0454800055', 1, '2020-11-06 08:45:02'),
(342, '258', '0454800035', 1, '2020-11-06 08:45:02'),
(343, '258', '0452100963', 1, '2020-11-06 08:45:02'),
(344, '258', '0153600039', 1, '2020-11-06 08:45:02'),
(345, '258', '0153600037', 1, '2020-11-06 08:45:02'),
(346, '258', '0113800503', 2, '2020-11-06 08:45:02'),
(347, '357', '15G6000002', 1, '2020-11-06 08:45:02'),
(348, '357', '16F1100006', 1, '2020-11-06 08:45:02'),
(349, '147', '0113800502', 2, '2020-11-06 08:48:55'),
(350, '9988', '15G0400001', 1, '2020-11-06 08:48:55'),
(351, '9988', '15G0300001', 1, '2020-11-06 08:48:55'),
(352, '9988', '15D4500002', 1, '2020-11-06 08:48:55'),
(353, '9988', '1596500141', 1, '2020-11-06 08:48:55'),
(354, '9988', '04D8000001', 1, '2020-11-06 08:48:55'),
(355, '9988', '0454800059', 1, '2020-11-06 08:48:55'),
(356, '9988', '0454800055', 1, '2020-11-06 08:48:55'),
(357, '9988', '0454800035', 1, '2020-11-06 08:48:55'),
(358, '9988', '0452100963', 1, '2020-11-06 08:48:55'),
(359, '9988', '0153600039', 1, '2020-11-06 08:48:55'),
(360, '9988', '0153600037', 1, '2020-11-06 08:48:55'),
(361, '9988', '0113800503', 2, '2020-11-06 08:48:55'),
(362, '9988', '0113800502', 2, '2020-11-06 08:48:55'),
(363, '147', '16F1100006', 1, '2020-11-06 08:48:55'),
(364, '147', '15G6000002', 1, '2020-11-06 08:48:55'),
(365, '147', '15G0400001', 1, '2020-11-06 08:48:55'),
(366, '147', '15G0300001', 1, '2020-11-06 08:48:55'),
(367, '147', '15D4500002', 1, '2020-11-06 08:48:55'),
(368, '147', '1596500141', 1, '2020-11-06 08:48:55'),
(369, '147', '04D8000001', 1, '2020-11-06 08:48:55'),
(370, '147', '0454800059', 1, '2020-11-06 08:48:55'),
(371, '147', '0454800055', 1, '2020-11-06 08:48:55'),
(372, '147', '0454800035', 1, '2020-11-06 08:48:55'),
(373, '147', '0452100963', 1, '2020-11-06 08:48:55'),
(374, '147', '0153600039', 1, '2020-11-06 08:48:55'),
(375, '147', '0153600037', 1, '2020-11-06 08:48:55'),
(376, '147', '0113800503', 2, '2020-11-06 08:48:55'),
(377, '9988', '15G6000002', 1, '2020-11-06 08:48:55'),
(378, '9988', '16F1100006', 1, '2020-11-06 08:48:55'),
(379, '369', '0113800502', 2, '2020-11-06 08:49:44'),
(380, '708', '15G0400001', 1, '2020-11-06 08:49:44'),
(381, '708', '15G0300001', 1, '2020-11-06 08:49:44'),
(382, '708', '15D4500002', 1, '2020-11-06 08:49:44'),
(383, '708', '1596500141', 1, '2020-11-06 08:49:44'),
(384, '708', '04D8000001', 1, '2020-11-06 08:49:44'),
(385, '708', '0454800059', 1, '2020-11-06 08:49:44'),
(386, '708', '0454800055', 1, '2020-11-06 08:49:44'),
(387, '708', '0454800035', 1, '2020-11-06 08:49:44'),
(388, '708', '0452100963', 1, '2020-11-06 08:49:44'),
(389, '708', '0153600039', 1, '2020-11-06 08:49:44'),
(390, '708', '0153600037', 1, '2020-11-06 08:49:44'),
(391, '708', '0113800503', 2, '2020-11-06 08:49:44'),
(392, '708', '0113800502', 2, '2020-11-06 08:49:44'),
(393, '369', '16F1100006', 1, '2020-11-06 08:49:44'),
(394, '369', '15G6000002', 1, '2020-11-06 08:49:44'),
(395, '369', '15G0400001', 1, '2020-11-06 08:49:44'),
(396, '369', '15G0300001', 1, '2020-11-06 08:49:44'),
(397, '369', '15D4500002', 1, '2020-11-06 08:49:44'),
(398, '369', '1596500141', 1, '2020-11-06 08:49:44'),
(399, '369', '04D8000001', 1, '2020-11-06 08:49:44'),
(400, '369', '0454800059', 1, '2020-11-06 08:49:44'),
(401, '369', '0454800055', 1, '2020-11-06 08:49:44'),
(402, '369', '0454800035', 1, '2020-11-06 08:49:44'),
(403, '369', '0452100963', 1, '2020-11-06 08:49:44'),
(404, '369', '0153600039', 1, '2020-11-06 08:49:44'),
(405, '369', '0153600037', 1, '2020-11-06 08:49:44'),
(406, '369', '0113800503', 2, '2020-11-06 08:49:44'),
(407, '708', '15G6000002', 1, '2020-11-06 08:49:44'),
(408, '708', '16F1100006', 1, '2020-11-06 08:49:44'),
(439, 'a123', '0113800502', 2, '2020-11-26 08:03:27'),
(440, 'a123', '0113800503', 2, '2020-11-26 08:03:27'),
(441, 'a123', '0153600037', 1, '2020-11-26 08:03:27'),
(442, 'a123', '0153600039', 1, '2020-11-26 08:03:27'),
(443, 'a123', '0452100963', 1, '2020-11-26 08:03:27'),
(444, 'a123', '0454800035', 1, '2020-11-26 08:03:27'),
(445, 'a123', '0454800055', 1, '2020-11-26 08:03:27'),
(446, 'a123', '0454800059', 1, '2020-11-26 08:03:27'),
(447, 'a123', '04D8000001', 1, '2020-11-26 08:03:27'),
(448, 'a123', '1596500141', 1, '2020-11-26 08:03:27'),
(449, 'a123', '15D4500002', 1, '2020-11-26 08:03:27'),
(450, 'a123', '15G0300001', 1, '2020-11-26 08:03:27'),
(451, 'a123', '15G0400001', 1, '2020-11-26 08:03:27'),
(452, 'a123', '15G6000002', 1, '2020-11-26 08:03:27'),
(453, 'a123', '16F1100006', 1, '2020-11-26 08:03:27'),
(454, 'AB123', '0113800502', 2, '2020-11-26 08:46:50'),
(455, 'AB456', '1596500141', 1, '2020-11-26 08:46:50'),
(456, 'AB456', '15D4500002', 1, '2020-11-26 08:46:50'),
(457, 'AB456', '15G0300001', 1, '2020-11-26 08:46:50'),
(458, 'AB456', '15G0400001', 1, '2020-11-26 08:46:50'),
(459, 'AB456', '15G6000002', 1, '2020-11-26 08:46:50'),
(460, 'AB456', '16F1100006', 1, '2020-11-26 08:46:50'),
(461, 'AB789', '0113800502', 2, '2020-11-26 08:46:50'),
(462, 'AB789', '0113800503', 2, '2020-11-26 08:46:50'),
(463, 'AB789', '0153600037', 1, '2020-11-26 08:46:50'),
(464, 'AB789', '0153600039', 1, '2020-11-26 08:46:50'),
(465, 'AB789', '0452100963', 1, '2020-11-26 08:46:50'),
(466, 'AB789', '0454800035', 1, '2020-11-26 08:46:50'),
(467, 'AB789', '0454800055', 1, '2020-11-26 08:46:50'),
(468, 'AB789', '0454800059', 1, '2020-11-26 08:46:50'),
(469, 'AB789', '04D8000001', 1, '2020-11-26 08:46:50'),
(470, 'AB789', '1596500141', 1, '2020-11-26 08:46:50'),
(471, 'AB789', '15D4500002', 1, '2020-11-26 08:46:50'),
(472, 'AB789', '15G0300001', 1, '2020-11-26 08:46:50'),
(473, 'AB789', '15G0400001', 1, '2020-11-26 08:46:50'),
(474, 'AB456', '04D8000001', 1, '2020-11-26 08:46:50'),
(475, 'AB789', '15G6000002', 1, '2020-11-26 08:46:50'),
(476, 'AB456', '0454800059', 1, '2020-11-26 08:46:50'),
(477, 'AB456', '0454800035', 1, '2020-11-26 08:46:50'),
(478, 'AB123', '0113800503', 2, '2020-11-26 08:46:50'),
(479, 'AB123', '0153600037', 1, '2020-11-26 08:46:50'),
(480, 'AB123', '0153600039', 1, '2020-11-26 08:46:50'),
(481, 'AB123', '0452100963', 1, '2020-11-26 08:46:50'),
(482, 'AB123', '0454800035', 1, '2020-11-26 08:46:50'),
(483, 'AB123', '0454800055', 1, '2020-11-26 08:46:50'),
(484, 'AB123', '0454800059', 1, '2020-11-26 08:46:50'),
(485, 'AB123', '04D8000001', 1, '2020-11-26 08:46:50'),
(486, 'AB123', '1596500141', 1, '2020-11-26 08:46:50'),
(487, 'AB123', '15D4500002', 1, '2020-11-26 08:46:50'),
(488, 'AB123', '15G0300001', 1, '2020-11-26 08:46:50'),
(489, 'AB123', '15G0400001', 1, '2020-11-26 08:46:50'),
(490, 'AB123', '15G6000002', 1, '2020-11-26 08:46:50'),
(491, 'AB123', '16F1100006', 1, '2020-11-26 08:46:50'),
(492, 'AB456', '0113800502', 2, '2020-11-26 08:46:50'),
(493, 'AB456', '0113800503', 2, '2020-11-26 08:46:50'),
(494, 'AB456', '0153600037', 1, '2020-11-26 08:46:50'),
(495, 'AB456', '0153600039', 1, '2020-11-26 08:46:50'),
(496, 'AB456', '0452100963', 1, '2020-11-26 08:46:50'),
(497, 'AB456', '0454800055', 1, '2020-11-26 08:46:50'),
(498, 'AB789', '16F1100006', 1, '2020-11-26 08:46:50'),
(499, 'ab2020', '0113800502', 2, '2020-12-15 07:11:57'),
(500, 'ab2020', '0113800503', 2, '2020-12-15 07:11:57'),
(501, 'ab2020', '0153600037', 1, '2020-12-15 07:11:57'),
(502, 'ab2020', '0153600039', 1, '2020-12-15 07:11:57'),
(503, 'ab2020', '0452100963', 1, '2020-12-15 07:11:57'),
(504, 'ab2020', '0454800035', 1, '2020-12-15 07:11:57'),
(505, 'ab2020', '0454800055', 1, '2020-12-15 07:11:57'),
(506, 'ab2020', '0454800059', 1, '2020-12-15 07:11:57'),
(507, 'ab2020', '04D8000001', 1, '2020-12-15 07:11:57'),
(508, 'ab2020', '1596500141', 1, '2020-12-15 07:11:57'),
(509, 'ab2020', '15D4500002', 1, '2020-12-15 07:11:57'),
(510, 'ab2020', '15G0300001', 1, '2020-12-15 07:11:57'),
(511, 'ab2020', '15G0400001', 1, '2020-12-15 07:11:57'),
(512, 'ab2020', '15G6000002', 1, '2020-12-15 07:11:57'),
(513, 'ab2020', '16F1100006', 1, '2020-12-15 07:11:57'),
(514, '123abc', '0113800502', 2, '2021-02-24 06:44:16'),
(515, '456def', '15G0400001', 1, '2021-02-24 06:44:16'),
(516, '456def', '15G0300001', 1, '2021-02-24 06:44:16'),
(517, '456def', '15D4500002', 1, '2021-02-24 06:44:16'),
(518, '456def', '1596500141', 1, '2021-02-24 06:44:16'),
(519, '456def', '04D8000001', 1, '2021-02-24 06:44:16'),
(520, '456def', '0454800059', 1, '2021-02-24 06:44:16'),
(521, '456def', '0454800055', 1, '2021-02-24 06:44:16'),
(522, '456def', '0454800035', 1, '2021-02-24 06:44:16'),
(523, '456def', '0452100963', 1, '2021-02-24 06:44:16'),
(524, '456def', '0153600039', 1, '2021-02-24 06:44:16'),
(525, '456def', '0153600037', 1, '2021-02-24 06:44:16'),
(526, '456def', '0113800503', 2, '2021-02-24 06:44:16'),
(527, '456def', '0113800502', 2, '2021-02-24 06:44:16'),
(528, '123abc', '16F1100006', 1, '2021-02-24 06:44:16'),
(529, '123abc', '15G6000002', 1, '2021-02-24 06:44:16'),
(530, '123abc', '15G0400001', 1, '2021-02-24 06:44:16'),
(531, '123abc', '15G0300001', 1, '2021-02-24 06:44:16'),
(532, '123abc', '15D4500002', 1, '2021-02-24 06:44:16'),
(533, '123abc', '1596500141', 1, '2021-02-24 06:44:16'),
(534, '123abc', '04D8000001', 1, '2021-02-24 06:44:16'),
(535, '123abc', '0454800059', 1, '2021-02-24 06:44:16'),
(536, '123abc', '0454800055', 1, '2021-02-24 06:44:16'),
(537, '123abc', '0454800035', 1, '2021-02-24 06:44:16'),
(538, '123abc', '0452100963', 1, '2021-02-24 06:44:16'),
(539, '123abc', '0153600039', 1, '2021-02-24 06:44:16'),
(540, '123abc', '0153600037', 1, '2021-02-24 06:44:16'),
(541, '123abc', '0113800503', 2, '2021-02-24 06:44:16'),
(542, '456def', '15G6000002', 1, '2021-02-24 06:44:16'),
(543, '456def', '16F1100006', 1, '2021-02-24 06:44:16');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_sell`
--

CREATE TABLE `wh_sell` (
  `sl_id` bigint(20) NOT NULL,
  `sl_flight_no` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `sl_flight_date` date NOT NULL,
  `sl_invoice_no` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `sl_customer_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `sl_passport_number` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `sl_nationality` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `sl_seat_number` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `sl_flight_number_detail` varchar(255) DEFAULT NULL,
  `sl_type_invoice` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Đang đổ dữ liệu cho bảng `wh_sell`
--

INSERT INTO `wh_sell` (`sl_id`, `sl_flight_no`, `sl_flight_date`, `sl_invoice_no`, `sl_customer_name`, `sl_passport_number`, `sl_nationality`, `sl_seat_number`, `sl_flight_number_detail`, `sl_type_invoice`) VALUES
(1604367551541, 'VJ801, VJ802', '2019-02-04', '8000', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', 'VJ801', NULL),
(1604367551574, 'VJ801, VJ802', '2019-02-04', '8000', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', 'VJ801', NULL),
(1604367551582, 'VJ801, VJ802', '2019-02-04', '8002', 'Nguyễn Văn C', 'A89052', 'VN', 'A14', 'VJ801', NULL),
(1604367551588, 'VJ801, VJ802', '2019-02-04', '8003', 'Nguyễn Văn D', 'A89053', 'VN', 'A15', 'VJ802', NULL),
(1604367551595, 'VJ808, VJ809', '2019-02-04', '8004', 'Nguyễn Văn E', 'A89054', 'VN', 'A16', 'VJ808', NULL),
(1604367551606, 'VJ808, VJ809', '2019-02-04', '8005', 'Nguyễn Văn F', 'A89055', 'VN', 'A17', 'VJ809', NULL),
(1604367551615, 'VJ811, VJ812', '2019-02-04', '8006', 'Nguyễn Văn G', 'A89056', 'VN', 'A18', 'VJ811', NULL),
(1604367551622, 'VJ811, VJ812', '2019-02-04', '8007', 'Nguyễn Văn H', 'A89057', 'VN', 'A19', 'VJ812', NULL),
(1604367551632, 'VJ828', '2019-02-04', '8010', 'Nguyễn Văn K', 'A89060', 'VN', 'A22', 'VJ828', NULL),
(1604367551640, 'VJ829', '2019-02-04', '8011', 'Nguyễn Văn L', 'A89061', 'VN', 'A23', 'VJ829', NULL),
(1604367551649, 'VJ860', '2019-02-04', '8012', 'Nguyễn Văn M', 'A89062', 'VN', 'A24', 'VJ860', NULL),
(1604367551656, 'VJ861', '2019-02-04', '8013', 'Nguyễn Văn N', 'A89063', 'VN', 'A25', 'VJ861', NULL),
(1604367551663, 'VJ932', '2019-02-04', '8014', 'Nguyễn Văn P', 'A89064', 'VN', 'A26', 'VJ932', NULL),
(1604367551671, 'VJ933', '2019-02-04', '8015', 'Nguyễn Văn Q', 'A89065', 'VN', 'A27', 'VJ933', NULL),
(1604387074704, 'VJ840, VJ841', '2020-11-03', '11032020', 'Đỗ Văn A', '987654321', 'VN', 'A12', 'VJ840', 0),
(1604652924867, 'VJ801, VJ802', '2020-11-06', '202041098', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', 'VJ801', 0),
(1604653027734, 'VJ801, VJ802', '2020-11-06', '202041100', 'Nguyễn Văn C', 'A89052', 'Viet nam', 'A14', 'VJ801', 0),
(1604653166787, 'VJ801, VJ802', '2020-11-06', '202041101', 'Nguyễn Văn D', 'A89053', 'Viet nam', 'A15', ' VJ802', 1),
(1604655193172, 'VJ808, VJ809', '2020-11-06', '202041102', 'Nguyễn Văn E', 'A89054', 'Viet nam', 'A16', 'VJ808', 1),
(1604655258957, 'VJ808, VJ809', '2020-11-06', '202041103', 'Nguyễn Văn F', 'A89055', 'Viet nam', 'A17', ' VJ809', 0),
(1604657022552, 'VJ811, VJ812', '2020-11-06', '202041104', 'Nguyễn Văn G', 'A89056', 'Viet nam', 'A18', 'VJ811', 0),
(1604657130064, 'VJ811, VJ812', '2020-11-06', '202041105', 'Nguyễn Văn H', 'A89057', 'Viet nam', 'A19', ' VJ812', 1),
(1604657211804, 'VJ940, VJ941', '2020-11-06', '202041106', 'Nguyễn Văn I', 'A89058', 'Viet nam', 'A20', 'VJ940', 0),
(1604657276374, 'VJ940, VJ941', '2020-11-06', '202041107', 'Nguyễn Văn J', 'A89059', 'Viet nam', 'A21', ' VJ941', 1),
(1604659101434, 'VJ828', '2020-11-06', '202041108', 'Nguyễn Văn K', 'A89060', 'Viet nam', 'A22', 'VJ828', 0),
(1604659196298, 'VJ829', '2020-11-06', '202041109', 'Nguyễn Văn L', 'A89061', 'Viet nam', 'A23', 'VJ829', 1),
(1604659737572, 'VJ861', '2020-11-06', '202041111', 'Nguyễn Văn N', 'A89063', 'Viet nam', 'a25', 'VJ861', 1),
(1604659816883, 'VJ932', '2020-11-06', '202041112', 'Nguyễn Văn P', 'A89064', 'Viet nam', 'A26', 'VJ932', 0);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_sell_detail`
--

CREATE TABLE `wh_sell_detail` (
  `sdt_id` int(11) NOT NULL,
  `sl_id` bigint(20) NOT NULL,
  `product_code` varchar(100) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `sdt_sold_number` int(11) NOT NULL,
  `sdt_currency` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL,
  `sdt_price` double DEFAULT NULL,
  `flight_number` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Đang đổ dữ liệu cho bảng `wh_sell_detail`
--

INSERT INTO `wh_sell_detail` (`sdt_id`, `sl_id`, `product_code`, `sdt_sold_number`, `sdt_currency`, `sdt_price`, `flight_number`) VALUES
(1, 1604367551541, '0452100963', 1, 'VNĐ', 1175550, NULL),
(2, 1604367551574, '04D8000001', 1, 'VNĐ', 691500, NULL),
(3, 1604367551582, '0141500255', 1, 'USD', 31.2, NULL),
(4, 1604367551588, '0114400314', 1, 'VNĐ', 746820, NULL),
(5, 1604367551595, '0242301548', 1, 'VNĐ', 484050, NULL),
(6, 1604367551606, '0454800060', 1, 'VNĐ', 622350, NULL),
(7, 1604367551615, '0113800503', 1, 'VNĐ', 691500, NULL),
(8, 1604367551622, '0113800502', 1, 'VNĐ', 843630, NULL),
(9, 1604367551632, '0153600037', 1, 'VNĐ', 843630, NULL),
(10, 1604367551640, '0153600039', 1, 'VNĐ', 1134060, NULL),
(11, 1604367551649, '0454800035', 1, 'VNĐ', 760650, NULL),
(12, 1604367551656, '0454800055', 1, 'VNĐ', 442560, NULL),
(13, 1604367551663, '16F1100006', 1, 'VNĐ', 207450, NULL),
(14, 1604367551671, '15G0300001', 1, 'VNĐ', 760650, NULL),
(15, 1604387074704, '16F1100006', 1, 'USD', 9, NULL),
(16, 1604652924867, '0113800502', 1, 'VND', 843630, NULL),
(17, 1604652924867, '0113800503', 1, 'VND', 691500, NULL),
(18, 1604653027734, '0153600037', 1, 'USD', 36.6, NULL),
(19, 1604653166787, '0153600039', 1, 'VND', 843630, NULL),
(21, 1604655193172, '0454800035', 1, 'VND', 843630, NULL),
(22, 1604655258957, '0452100963', 1, 'VND', 1134060, NULL),
(23, 1604657022552, '0454800055', 1, 'VND', 760650, NULL),
(24, 1604657130064, '04D8000001', 1, 'VND', 1175550, NULL),
(25, 1604657211804, '04D8000001', 1, 'VND', 442560, NULL),
(26, 1604657276374, '16F1100006', 1, 'VND', 553200, NULL),
(27, 1604659101434, '15G6000002', 1, 'VND', 691500, NULL),
(28, 1604659196298, '15G0300001', 1, 'VND', 207450, NULL),
(29, 1604659737572, '1596500141', 1, 'VND', 760650, NULL),
(30, 1604659816883, '15G0400001', 1, 'VND', 2475570, NULL);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_user`
--

CREATE TABLE `wh_user` (
  `user_id` int(11) NOT NULL,
  `user_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_password` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_salt` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `user_full_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `user_enable` bit(1) NOT NULL DEFAULT b'1',
  `user_created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `user_department_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_user`
--

INSERT INTO `wh_user` (`user_id`, `user_name`, `user_password`, `user_salt`, `user_full_name`, `user_enable`, `user_created_at`, `user_department_id`) VALUES
(1, 'backend', '1515E280EE877059667B63A766FA2B060EDA2A473D4518A11FAD69A32B1C33ECC27B0800A458ED733F4D65DCEEF83E936CE0EB5AFD7A517C4C27D96AE6C5C343', '45693129', 'Nguyen Backend', b'0', '2020-02-26 00:11:01', 1),
(2, 'frontend', '1BB211185D552870BF77F1C445ADCEC2731347967182D63EE4940E9C3623D6708AF1FF9B02EA4979F39BC099B817C48D2CE860FD4C8AAE02626F3C8D3611AC92', '56186786', 'Nguyen Frontend', b'0', '2020-02-26 00:10:58', 1),
(3, 'admin', '755067EDA539DDF4E48B5A8C979D9A3B21904963D52E5657B08470A6541F1D7C4C03B804EF90981CD44BCA66A88B8D9F6EA83251B0D433561610A56B4834DB94', '63550449', 'Administrator', b'1', '2020-02-18 09:53:52', 1),
(4, 'tuanngoanh@vietjetair.com', '7D1614E570238537C47CE8B32B12F4991BD2049D985ACEB6911D3CD9988DBCB201C6E225A7B4FA420B289A640F432FC521D4E0057699179BC51D5EE4D7BD2219', '77365398', 'Ngô Anh Tuấn', b'0', '2020-10-30 08:06:12', 2),
(5, 'huonghuynh@vietjetair.com', '43D8C6F45EB4BF5F6FF46C035AFC399A2333B530DECCC3EF12B8F5CE222F1FC8BD7408A4E17EB33FC93420B83066657E10BDC9ACD326E3926A31D817DF1551FA', '46898184', 'Huỳnh Thị Thu Hương', b'0', '2020-10-30 08:16:23', 2),
(6, 'tuanngoanh@vietjetair.com', '3C7DFAFF613668B85DA3F0BACF5B2A4A1626811E59E95144A950E3B994629DD99859982938F5C823C2D297CD5EA19EE095BA583B05B05380A5F88096C0A34983', 'vggcgoTe', 'Ngô Anh Tuấn', b'1', '2020-10-30 08:06:58', 2),
(7, 'vungu', '49DA6C2187A5D8D73A470F56BD25ADCFB102A39A2E56D471C75C129D8B264075817616C1AF5798B0B83ED0B31E689C3138715251621A1F7B59F009987EB823F1', 'jotnoRlb', 'Nguyen backend', b'0', '2020-11-10 05:26:50', 1),
(8, 'luyen.le@alta.com.vn', 'BF70558C936A2429CF2083034E4240500899E1E6E6A4AC75AA1FFE5A58D14EAB9FD734FA9F9B0FD8FCD15C69B8BA77D335F5D1EA051E8B6C1266D74C131CC197', 'tgILJMGW', 'luyen  backend', b'1', '2020-11-10 05:27:59', 1);

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_user_permission`
--

CREATE TABLE `wh_user_permission` (
  `user_permission_id` int(11) NOT NULL,
  `user_permission_userid` int(11) DEFAULT NULL,
  `user_permission_permissionid` int(11) DEFAULT NULL,
  `user_permission_permissioncode` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Đang đổ dữ liệu cho bảng `wh_user_permission`
--

INSERT INTO `wh_user_permission` (`user_permission_id`, `user_permission_userid`, `user_permission_permissionid`, `user_permission_permissioncode`) VALUES
(2, 1, 24, 'QLND_DSND'),
(3, 1, 28, 'QLND_PQCN'),
(4, 3, 24, 'QLND_DSND'),
(5, 3, 1, 'QLTK_DSTK'),
(6, 3, 9, 'QLDC_NHDB'),
(7, 3, 8, 'QLDC_TDXH'),
(8, 3, 7, 'QLDC_LSHU'),
(9, 3, 6, 'QLDC_NDLB'),
(10, 3, 5, 'QLDC_NDDC'),
(11, 3, 4, 'QLDC_XDDC'),
(12, 3, 15, 'BACA_TTTK'),
(13, 3, 14, 'BACA_QTKD'),
(14, 3, 13, 'BACA_BK05'),
(15, 3, 12, 'BACA_BCTK'),
(16, 3, 11, 'BACA_CTHN'),
(17, 3, 10, 'BACA_CTHX'),
(18, 3, 20, 'DMSP_TMBH'),
(19, 3, 19, 'DMSP_XOSP'),
(20, 3, 18, 'DMSP_CNSP'),
(21, 3, 17, 'DMSP_TSPM'),
(22, 3, 16, 'DMSP_XDSP'),
(23, 3, 23, 'QLCB_CNCB'),
(24, 3, 22, 'QLCB_TDBM'),
(25, 3, 21, 'QLCB_DSCB'),
(26, 3, 28, 'QLND_PQCN'),
(27, 3, 27, 'QLND_DSBP'),
(28, 3, 26, 'QLND_CNND'),
(29, 3, 25, 'QLND_TNDM'),
(30, 3, 2, 'QLTK_TTKN'),
(31, 3, 3, 'QLTK_TTKX'),
(32, 3, 33, 'QLND_TMBP'),
(33, 3, 35, 'QLND_XOBP'),
(34, 3, 36, 'QLND_XOND'),
(35, 3, 32, 'DMSP_IPSP'),
(36, 3, 30, 'QLDC_DCHN'),
(37, 3, 31, 'QLDC_CNHN'),
(38, 4, 21, 'QLCB_DSCB'),
(39, 4, 2, 'QLTK_TTKN'),
(40, 4, 1, 'QLTK_DSTK'),
(41, 4, 31, 'QLDC_CNHN'),
(42, 4, 30, 'QLDC_DCHN'),
(43, 4, 9, 'QLDC_NHDB'),
(44, 4, 8, 'QLDC_TDXH'),
(45, 4, 7, 'QLDC_LSHU'),
(46, 4, 6, 'QLDC_NDLB'),
(47, 4, 5, 'QLDC_NDDC'),
(48, 4, 4, 'QLDC_XDDC'),
(49, 4, 15, 'BACA_TTTK'),
(50, 4, 3, 'QLTK_TTKX'),
(51, 4, 14, 'BACA_QTKD'),
(52, 4, 12, 'BACA_BCTK'),
(53, 4, 11, 'BACA_CTHN'),
(54, 4, 10, 'BACA_CTHX'),
(55, 4, 32, 'DMSP_IPSP'),
(56, 4, 20, 'DMSP_TMBH'),
(57, 4, 19, 'DMSP_XOSP'),
(58, 4, 18, 'DMSP_CNSP'),
(59, 4, 17, 'DMSP_TSPM'),
(60, 4, 16, 'DMSP_XDSP'),
(61, 4, 23, 'QLCB_CNCB'),
(62, 4, 22, 'QLCB_TDBM'),
(63, 4, 13, 'BACA_BK05'),
(64, 4, 24, 'QLND_DSND'),
(65, 5, 24, 'QLND_DSND'),
(66, 5, 1, 'QLTK_DSTK'),
(67, 5, 31, 'QLDC_CNHN'),
(68, 5, 30, 'QLDC_DCHN'),
(69, 5, 9, 'QLDC_NHDB'),
(70, 5, 8, 'QLDC_TDXH'),
(71, 5, 7, 'QLDC_LSHU'),
(72, 5, 6, 'QLDC_NDLB'),
(73, 5, 5, 'QLDC_NDDC'),
(74, 5, 4, 'QLDC_XDDC'),
(75, 5, 15, 'BACA_TTTK'),
(76, 5, 14, 'BACA_QTKD'),
(77, 5, 13, 'BACA_BK05'),
(78, 5, 12, 'BACA_BCTK'),
(79, 5, 11, 'BACA_CTHN'),
(80, 5, 10, 'BACA_CTHX'),
(81, 5, 32, 'DMSP_IPSP'),
(82, 5, 20, 'DMSP_TMBH'),
(83, 5, 25, 'QLND_TNDM'),
(84, 5, 26, 'QLND_CNND'),
(85, 5, 27, 'QLND_DSBP'),
(86, 5, 28, 'QLND_PQCN'),
(87, 5, 33, 'QLND_TMBP'),
(88, 5, 35, 'QLND_XOBP'),
(89, 5, 2, 'QLTK_TTKN'),
(90, 5, 36, 'QLND_XOND'),
(91, 5, 22, 'QLCB_TDBM'),
(92, 5, 23, 'QLCB_CNCB'),
(93, 5, 16, 'DMSP_XDSP'),
(94, 5, 17, 'DMSP_TSPM'),
(95, 5, 18, 'DMSP_CNSP'),
(96, 5, 19, 'DMSP_XOSP'),
(97, 5, 21, 'QLCB_DSCB'),
(98, 5, 3, 'QLTK_TTKX'),
(99, 4, 25, 'QLND_TNDM'),
(100, 4, 26, 'QLND_CNND'),
(101, 4, 27, 'QLND_DSBP'),
(102, 4, 28, 'QLND_PQCN'),
(103, 4, 33, 'QLND_TMBP'),
(104, 4, 35, 'QLND_XOBP'),
(105, 4, 36, 'QLND_XOND'),
(106, 6, 24, 'QLND_DSND'),
(107, 6, 1, 'QLTK_DSTK'),
(108, 6, 31, 'QLDC_CNHN'),
(109, 6, 30, 'QLDC_DCHN'),
(110, 6, 9, 'QLDC_NHDB'),
(111, 6, 8, 'QLDC_TDXH'),
(112, 6, 7, 'QLDC_LSHU'),
(113, 6, 6, 'QLDC_NDLB'),
(114, 6, 5, 'QLDC_NDDC'),
(115, 6, 4, 'QLDC_XDDC'),
(116, 6, 15, 'BACA_TTTK'),
(117, 6, 14, 'BACA_QTKD'),
(118, 6, 13, 'BACA_BK05'),
(119, 6, 12, 'BACA_BCTK'),
(120, 6, 11, 'BACA_CTHN'),
(121, 6, 10, 'BACA_CTHX'),
(122, 6, 32, 'DMSP_IPSP'),
(123, 6, 20, 'DMSP_TMBH'),
(124, 6, 25, 'QLND_TNDM'),
(125, 6, 26, 'QLND_CNND'),
(126, 6, 27, 'QLND_DSBP'),
(127, 6, 28, 'QLND_PQCN'),
(128, 6, 33, 'QLND_TMBP'),
(129, 6, 35, 'QLND_XOBP'),
(130, 6, 2, 'QLTK_TTKN'),
(131, 6, 36, 'QLND_XOND'),
(132, 6, 22, 'QLCB_TDBM'),
(133, 6, 23, 'QLCB_CNCB'),
(134, 6, 16, 'DMSP_XDSP'),
(135, 6, 17, 'DMSP_TSPM'),
(136, 6, 18, 'DMSP_CNSP'),
(137, 6, 19, 'DMSP_XOSP'),
(138, 6, 21, 'QLCB_DSCB'),
(139, 6, 3, 'QLTK_TTKX'),
(140, 6, 24, 'QLND_DSND'),
(141, 6, 5, 'QLDC_NDDC'),
(142, 6, 4, 'QLDC_XDDC'),
(143, 6, 15, 'BACA_TTTK'),
(144, 6, 14, 'BACA_QTKD'),
(145, 6, 13, 'BACA_BK05'),
(146, 6, 12, 'BACA_BCTK'),
(147, 6, 11, 'BACA_CTHN'),
(148, 6, 10, 'BACA_CTHX'),
(149, 6, 32, 'DMSP_IPSP'),
(150, 6, 20, 'DMSP_TMBH'),
(151, 6, 19, 'DMSP_XOSP'),
(152, 6, 6, 'QLDC_NDLB'),
(153, 6, 18, 'DMSP_CNSP'),
(154, 6, 16, 'DMSP_XDSP'),
(155, 6, 23, 'QLCB_CNCB'),
(156, 6, 22, 'QLCB_TDBM'),
(157, 6, 21, 'QLCB_DSCB'),
(158, 6, 36, 'QLND_XOND'),
(159, 6, 35, 'QLND_XOBP'),
(160, 6, 33, 'QLND_TMBP'),
(161, 6, 28, 'QLND_PQCN'),
(162, 6, 27, 'QLND_DSBP'),
(163, 6, 26, 'QLND_CNND'),
(164, 6, 25, 'QLND_TNDM'),
(165, 6, 17, 'DMSP_TSPM'),
(166, 6, 7, 'QLDC_LSHU');

-- --------------------------------------------------------

--
-- Cấu trúc bảng cho bảng `wh_warehouse`
--

CREATE TABLE `wh_warehouse` (
  `de_number` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `warehouse_id` int(11) NOT NULL,
  `warehouse_quantity` int(11) DEFAULT NULL,
  `warehouse_weight` float DEFAULT NULL,
  `warehouse_pos` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `warehouse_note` mediumtext COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Chỉ mục cho các bảng đã đổ
--

--
-- Chỉ mục cho bảng `wh_airport`
--
ALTER TABLE `wh_airport`
  ADD PRIMARY KEY (`airport_code`);

--
-- Chỉ mục cho bảng `wh_cityPair`
--
ALTER TABLE `wh_cityPair`
  ADD PRIMARY KEY (`citypair_id`);

--
-- Chỉ mục cho bảng `wh_copy_seal`
--
ALTER TABLE `wh_copy_seal`
  ADD PRIMARY KEY (`citypair_id`);

--
-- Chỉ mục cho bảng `wh_declaration`
--
ALTER TABLE `wh_declaration`
  ADD PRIMARY KEY (`de_number`);

--
-- Chỉ mục cho bảng `wh_declaration_extension`
--
ALTER TABLE `wh_declaration_extension`
  ADD PRIMARY KEY (`declaration_extension_id`);

--
-- Chỉ mục cho bảng `wh_department`
--
ALTER TABLE `wh_department`
  ADD PRIMARY KEY (`department_id`);

--
-- Chỉ mục cho bảng `wh_department_permissions`
--
ALTER TABLE `wh_department_permissions`
  ADD PRIMARY KEY (`department_permission_id`),
  ADD KEY `fk_wh_department_permissions_wh_department` (`department_permission_departmentid`),
  ADD KEY `fk_wh_department_permissions_wh_permissions` (`department_permission_permissionid`);

--
-- Chỉ mục cho bảng `wh_destroy`
--
ALTER TABLE `wh_destroy`
  ADD PRIMARY KEY (`destroy_id`);

--
-- Chỉ mục cho bảng `wh_destroy_detail`
--
ALTER TABLE `wh_destroy_detail`
  ADD PRIMARY KEY (`destroy_detail_id`);

--
-- Chỉ mục cho bảng `wh_de_details`
--
ALTER TABLE `wh_de_details`
  ADD PRIMARY KEY (`dt_id`);

--
-- Chỉ mục cho bảng `wh_exchangerates`
--
ALTER TABLE `wh_exchangerates`
  ADD PRIMARY KEY (`exchangerate_id`);

--
-- Chỉ mục cho bảng `wh_file_upload`
--
ALTER TABLE `wh_file_upload`
  ADD PRIMARY KEY (`file_id`);

--
-- Chỉ mục cho bảng `wh_inventory`
--
ALTER TABLE `wh_inventory`
  ADD PRIMARY KEY (`in_id`);

--
-- Chỉ mục cho bảng `wh_menus`
--
ALTER TABLE `wh_menus`
  ADD PRIMARY KEY (`menu_id`);

--
-- Chỉ mục cho bảng `wh_menu_details`
--
ALTER TABLE `wh_menu_details`
  ADD PRIMARY KEY (`menu_detail_id`);

--
-- Chỉ mục cho bảng `wh_modules`
--
ALTER TABLE `wh_modules`
  ADD PRIMARY KEY (`module_id`);

--
-- Chỉ mục cho bảng `wh_permissions`
--
ALTER TABLE `wh_permissions`
  ADD PRIMARY KEY (`permission_id`),
  ADD KEY `fk_wh_permissions_wh_modules` (`permission_module_id`);

--
-- Chỉ mục cho bảng `wh_products`
--
ALTER TABLE `wh_products`
  ADD PRIMARY KEY (`product_code`);

--
-- Chỉ mục cho bảng `wh_seal`
--
ALTER TABLE `wh_seal`
  ADD PRIMARY KEY (`se_number`);

--
-- Chỉ mục cho bảng `wh_seal_detail`
--
ALTER TABLE `wh_seal_detail`
  ADD PRIMARY KEY (`sealdetail_id`);

--
-- Chỉ mục cho bảng `wh_seal_product`
--
ALTER TABLE `wh_seal_product`
  ADD PRIMARY KEY (`id`);

--
-- Chỉ mục cho bảng `wh_sell`
--
ALTER TABLE `wh_sell`
  ADD PRIMARY KEY (`sl_id`);

--
-- Chỉ mục cho bảng `wh_sell_detail`
--
ALTER TABLE `wh_sell_detail`
  ADD PRIMARY KEY (`sdt_id`);

--
-- Chỉ mục cho bảng `wh_user`
--
ALTER TABLE `wh_user`
  ADD PRIMARY KEY (`user_id`);

--
-- Chỉ mục cho bảng `wh_user_permission`
--
ALTER TABLE `wh_user_permission`
  ADD PRIMARY KEY (`user_permission_id`),
  ADD KEY `fk_wh_user_permission_wh_user` (`user_permission_userid`),
  ADD KEY `fk_wh_user_permission_wh_permissions` (`user_permission_permissionid`);

--
-- Chỉ mục cho bảng `wh_warehouse`
--
ALTER TABLE `wh_warehouse`
  ADD PRIMARY KEY (`warehouse_id`);

--
-- AUTO_INCREMENT cho các bảng đã đổ
--

--
-- AUTO_INCREMENT cho bảng `wh_cityPair`
--
ALTER TABLE `wh_cityPair`
  MODIFY `citypair_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT cho bảng `wh_department`
--
ALTER TABLE `wh_department`
  MODIFY `department_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT cho bảng `wh_department_permissions`
--
ALTER TABLE `wh_department_permissions`
  MODIFY `department_permission_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT cho bảng `wh_destroy`
--
ALTER TABLE `wh_destroy`
  MODIFY `destroy_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT cho bảng `wh_destroy_detail`
--
ALTER TABLE `wh_destroy_detail`
  MODIFY `destroy_detail_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT cho bảng `wh_de_details`
--
ALTER TABLE `wh_de_details`
  MODIFY `dt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- AUTO_INCREMENT cho bảng `wh_file_upload`
--
ALTER TABLE `wh_file_upload`
  MODIFY `file_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT cho bảng `wh_inventory`
--
ALTER TABLE `wh_inventory`
  MODIFY `in_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT cho bảng `wh_menus`
--
ALTER TABLE `wh_menus`
  MODIFY `menu_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT cho bảng `wh_menu_details`
--
ALTER TABLE `wh_menu_details`
  MODIFY `menu_detail_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;

--
-- AUTO_INCREMENT cho bảng `wh_modules`
--
ALTER TABLE `wh_modules`
  MODIFY `module_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT cho bảng `wh_permissions`
--
ALTER TABLE `wh_permissions`
  MODIFY `permission_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT cho bảng `wh_seal_detail`
--
ALTER TABLE `wh_seal_detail`
  MODIFY `sealdetail_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=418;

--
-- AUTO_INCREMENT cho bảng `wh_seal_product`
--
ALTER TABLE `wh_seal_product`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=544;

--
-- AUTO_INCREMENT cho bảng `wh_sell_detail`
--
ALTER TABLE `wh_sell_detail`
  MODIFY `sdt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=31;

--
-- AUTO_INCREMENT cho bảng `wh_user`
--
ALTER TABLE `wh_user`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT cho bảng `wh_user_permission`
--
ALTER TABLE `wh_user_permission`
  MODIFY `user_permission_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=167;

--
-- AUTO_INCREMENT cho bảng `wh_warehouse`
--
ALTER TABLE `wh_warehouse`
  MODIFY `warehouse_id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
