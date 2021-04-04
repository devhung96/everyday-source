-- phpMyAdmin SQL Dump
-- version 5.0.1
-- https://www.phpmyadmin.net/
--
-- Host: 192.168.11.42
-- Generation Time: Sep 25, 2020 at 01:04 PM
-- Server version: 10.4.11-MariaDB-1:10.4.11+maria~bionic
-- PHP Version: 7.2.19-0ubuntu0.18.04.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `dutyfree_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `wh_airport`
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
-- Dumping data for table `wh_airport`
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
-- Table structure for table `wh_cityPair`
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
-- Dumping data for table `wh_cityPair`
--

INSERT INTO `wh_cityPair` (`citypair_id`, `citypair_route`, `citypair_status`, `citypair_date_start`, `citypair_schedule`, `created_at`, `updated_at`, `order`) VALUES
(1, 'SGN-BKK', 1, '2019-01-29 17:00:00', '[{\"Departure\":\"SGN\",\"Arrival\":\"BKK\",\"Route\":null,\"TypeSchedule\":0,\"FlightNumber\":\"VJ801\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"BKK\",\"Arrival\":\"SGN\",\"Route\":null,\"TypeSchedule\":1,\"FlightNumber\":\"VJ802\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"SGN\",\"Arrival\":\"BKK\",\"Route\":null,\"TypeSchedule\":0,\"FlightNumber\":\"VJ803\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"BKK\",\"Arrival\":\"SGN\",\"Route\":null,\"TypeSchedule\":1,\"FlightNumber\":\"VJ804\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"SGN\",\"Arrival\":\"BKK\",\"Route\":null,\"TypeSchedule\":0,\"FlightNumber\":\"VJ805\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"BKK\",\"Arrival\":\"SGN\",\"Route\":null,\"TypeSchedule\":1,\"FlightNumber\":\"VJ806\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:22', '2020-08-18 09:10:22', 2),
(2, 'SGN-HKT', 1, '2019-01-29 17:00:00', '[{\"Departure\":\"HKT\",\"Arrival\":\"SGN\",\"Route\":null,\"TypeSchedule\":1,\"FlightNumber\":\"VJ808\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]},{\"Departure\":\"SGN\",\"Arrival\":\"HKT\",\"Route\":null,\"TypeSchedule\":0,\"FlightNumber\":\"VJ809\",\"Status\":1,\"FlightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(3, 'SGN-SIN', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"SIN\",\"route\":\"SGN-SIN\",\"typeSchedule\":0,\"flightNumber\":\"VJ811\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"SIN\",\"arrival\":\"SGN\",\"route\":\"SIN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ812\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\"]},{\"departure\":\"SGN\",\"arrival\":\"SIN\",\"route\":\"SGN-SIN\",\"typeSchedule\":0,\"flightNumber\":\"VJ813\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"SIN\",\"arrival\":\"SGN\",\"route\":\"SIN-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ814\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(4, 'SGN-NRT', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"NRT\",\"route\":\"SGN-NRT\",\"typeSchedule\":0,\"flightNumber\":\"VJ822\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"NRT\",\"arrival\":\"SGN\",\"route\":\"NRT-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ823\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(5, 'SGN-KUL', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"KUL\",\"route\":\"SGN-KUL\",\"typeSchedule\":0,\"flightNumber\":\"VJ825\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"KUL\",\"arrival\":\"SGN\",\"route\":\"KUL-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ826\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
(6, 'SGN-KIX', 1, '2020-08-18 09:10:23', '[{\"departure\":\"SGN\",\"arrival\":\"KIX\",\"route\":\"SGN-KIX\",\"typeSchedule\":0,\"flightNumber\":\"VJ828\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]},{\"departure\":\"KIX\",\"arrival\":\"SGN\",\"route\":\"KIX-SGN\",\"typeSchedule\":1,\"flightNumber\":\"VJ829\",\"status\":1,\"flightTime\":[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]}]', '2020-08-18 09:10:23', '2020-08-18 09:10:23', 2),
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
-- Table structure for table `wh_copy_seal`
--

CREATE TABLE `wh_copy_seal` (
  `citypair_id` int(11) NOT NULL,
  `data_copy` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`data_copy`))
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `wh_declaration`
--

CREATE TABLE `wh_declaration` (
  `de_number` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `de_type` int(11) NOT NULL,
  `de_date_re` datetime DEFAULT NULL,
  `de_status` int(11) DEFAULT 1,
  `de_parent_number` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_content` longtext COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_extended_dispatch` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_settlement_dispatch` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `de_renewal_date` datetime DEFAULT NULL,
  `de_settlement_status` int(11) NOT NULL DEFAULT 0,
  `de_new_date` datetime DEFAULT NULL,
  `user_id` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `wh_declaration`
--

INSERT INTO `wh_declaration` (`de_number`, `de_type`, `de_date_re`, `de_status`, `de_parent_number`, `de_content`, `de_extended_dispatch`, `de_settlement_dispatch`, `de_renewal_date`, `de_settlement_status`, `de_new_date`, `user_id`) VALUES
('10111997', 1, '2019-04-30 00:00:00', 0, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/04/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102059463330', 1, '2018-06-13 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"13/06/2019\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"24/09/2020\",\"importnumber\":\"\",\"supplier\":\"\",\"deliver\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102471574440', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"23/09/2020\",\"importnumber\":\"2019/001 A1\",\"supplier\":\"King Power Traveler\",\"deliver\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102471574441', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"27/08/2020\",\"importnumber\":\"2019/001 A\",\"supplier\":\"KPI\",\"deliver\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102471591721', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"30/01/2019\",\"importnumber\":\"2019/001 B\",\"supplier\":\"KPI\",\"deliver\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102471594740', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"30/01/2019\",\"importnumber\":\"2019/001 C\",\"supplier\":\"\",\"deliver\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102471601960', 1, '2019-01-30 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"30/01/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"30/01/2019\",\"importnumber\":\"2019/001 D\",\"supplier\":\"\",\"deliver\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('102633000410', 1, '2019-05-09 00:00:00', 1, '', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"09/05/2020\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateadded\":\"\",\"importnumber\":\"2019/002 A\",\"supplier\":\"King Power Traveler\",\"deliver\":\"VJA\"}', NULL, NULL, NULL, 0, NULL, 3),
('302653110560', 2, '2019-07-23 00:00:00', 1, '102471574440', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"102471574440\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"23/09/2020\",\"exportnumber\":\"2019/001.25\",\"requestname\":\"VietJet Air\",\"rebill\":\"\"}', NULL, NULL, NULL, 0, NULL, 3),
('303104918340', 2, '2020-09-08 00:00:00', 1, '102471574441', '{\"first\":\"\",\"classificationcheckcode\":\"\",\"temporarynumber\":\"102471574441\",\"typecode\":\"\",\"representcodedeclaration\":\"\",\"customsname\":\"\",\"changedatere\":\"\",\"parthandlingcode\":\"\",\"deadline\":\"\",\"userimport\":\"\",\"userexport\":\"\",\"waybillnumber\":\"\",\"quantity\":\"\",\"gross\":\"\",\"warehousrlocation\":\"\",\"finaldestination\":\"\",\"queuingplace\":\"\",\"opiniontransportation\":\"\",\"dateestimateddelivery\":\"\",\"symbolsandnumbers\":\"\",\"billnumber\":\"\",\"releasedate\":\"\",\"paymentmethods\":\"\",\"electronicbillnumber\":\"\",\"totalbillvalue\":\"\",\"totaltaxablevalue\":\"\",\"totalvaluedistributioncoefficient\":\"\",\"contentverificationcode\":\"\",\"note\":\"\",\"dateexported\":\"08/09/2020\",\"exportnumber\":\"\",\"requestname\":\"\",\"rebill\":\"\"}', NULL, NULL, NULL, 0, NULL, 3);

-- --------------------------------------------------------

--
-- Table structure for table `wh_department`
--

CREATE TABLE `wh_department` (
  `department_id` int(11) NOT NULL,
  `department_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `department_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `department_enable` tinyint(1) NOT NULL,
  `department_created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `wh_department`
--

INSERT INTO `wh_department` (`department_id`, `department_name`, `department_code`, `department_enable`, `department_created_at`) VALUES
(1, 'System', 'SYSS', 1, '2020-02-17 08:44:21'),
(2, 'Ancillary', 'ANCI', 1, '2020-02-17 08:44:36'),
(3, 'SCSC', 'SCSC', 1, '2020-02-17 08:44:42');

-- --------------------------------------------------------

--
-- Table structure for table `wh_department_permissions`
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
-- Table structure for table `wh_destroy`
--

CREATE TABLE `wh_destroy` (
  `destroy_id` int(11) NOT NULL,
  `destroy_request_date` datetime NOT NULL,
  `destroy_status` int(11) DEFAULT 1,
  `destroy_user` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `destroy_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='lịch sử hủy';

--
-- Dumping data for table `wh_destroy`
--

INSERT INTO `wh_destroy` (`destroy_id`, `destroy_request_date`, `destroy_status`, `destroy_user`, `destroy_code`) VALUES
(1, '2020-09-08 10:15:45', 1, 'admin', '1231231'),
(2, '2020-09-24 11:33:50', 1, 'admin', '05-20');

-- --------------------------------------------------------

--
-- Table structure for table `wh_destroy_detail`
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
-- Dumping data for table `wh_destroy_detail`
--

INSERT INTO `wh_destroy_detail` (`destroy_detail_id`, `de_number`, `destroy_id`, `product_code`, `destroy_detail_quantity`, `destroy_detail_note`, `product_price`) VALUES
(1, '102471594740', 1, '15G0300001', 3, 'Expired', 825),
(2, '102059463330', 2, '3101200069', 10, 'Không sử dụng', 3276);

-- --------------------------------------------------------

--
-- Table structure for table `wh_de_details`
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
-- Dumping data for table `wh_de_details`
--

INSERT INTO `wh_de_details` (`dt_id`, `de_number`, `product_code`, `dt_quantity`, `dt_invoice_price`, `dt_invoice_value`, `dt_product_number`, `dt_own_management_code`, `dt_code_re_confirm`) VALUES
(5, '102471574441', '0153600039', 22, 49.2, 1082.4, '0153600039', NULL, NULL),
(6, '102471574441', '0153600037', 22, 36.6, 805.2, '0153600037', NULL, NULL),
(7, '102471574441', '0113800502', 33, 33.6, 1108.8, '0113800502', NULL, NULL),
(8, '102471574441', '0113800503', 40, 30, 1200, '0113800503', NULL, NULL),
(9, '102471591721', '16F1100006', 15, 9, 135, '16F1100006', NULL, NULL),
(10, '102471591721', '04D8000001', 10, 30, 300, '04D8000001', NULL, NULL),
(11, '102471591721', '0454800059', 5, 24, 120, '0454800059', NULL, NULL),
(12, '102471591721', '0454800055', 24, 19.2, 460.79999999999995, '0454800055', NULL, NULL),
(13, '102471591721', '0452100963', 10, 51, 510, '0452100963', NULL, NULL),
(14, '102471591721', '0454800035', 20, 33, 660, '0454800035', NULL, NULL),
(15, '102471594740', '15G0400001', 18, 33, 594, '15G0400001', NULL, NULL),
(16, '102471594740', '1596500141', 15, 39, 585, '1596500141', NULL, NULL),
(17, '102471594740', '15D4500002', 6, 107.4, 644.4000000000001, '15D4500002', NULL, NULL),
(18, '102471594740', '15G0300001', 25, 33, 825, '15G0300001', NULL, NULL),
(19, '102471594740', '15G6000002', 6, 35.4, 212.39999999999998, '15G6000002', NULL, NULL),
(22, '102471601960', '1655500083', 20, 81, 1620, '1655500083', NULL, NULL),
(23, '102471601960', '16D2400017', 6, 51, 306, '16D2400017', NULL, NULL),
(25, '303104918340', '0113800503', 3, 30, 90, '01138000503', NULL, NULL),
(32, '102471574440', '0153600039', 22, 49.2, 1082.4, '3303000', NULL, NULL),
(33, '102471574440', '0153600037', 22, 36.6, 805.2, '3303', NULL, NULL),
(34, '102471574440', '0113800503', 40, 30, 1200, '33030000', NULL, NULL),
(35, '102471574440', '0113800502', 33, 36.6, 1207.8, '30030000', NULL, NULL),
(40, '302653110560', '0153600039', 22, 49.2, 1082.4, '3303000', NULL, NULL),
(41, '302653110560', '0153600037', 22, 36.6, 805.2, '3303', NULL, NULL),
(42, '302653110560', '0113800503', 40, 30, 1200, '33030000', NULL, NULL),
(43, '302653110560', '0113800502', 33, 36.6, 1207.8, '30030000', NULL, NULL),
(54, '102633000410', '0153600045', 75, 55.2, 4140, '33030000', NULL, NULL),
(55, '102633000410', '0153600048', 75, 66, 4950, '33030000', NULL, NULL),
(56, '102633000410', '0141500255', 80, 31.2, 2496, '33030000', NULL, NULL),
(57, '102633000410', '0113800527', 90, 39.6, 3564, '33030000', NULL, NULL),
(58, '102633000410', '01D2200004', 93, 6, 558, '33030000', NULL, NULL),
(59, '102633000410', '01E9000005', 30, 10.8, 324, '33049990', NULL, NULL),
(60, '102633000410', '0129100004', 30, 40.8, 1224, '33030000', NULL, NULL),
(61, '102633000410', '0118100045', 35, 36, 1260, '33030000', NULL, NULL),
(62, '102633000410', '0186200001', 75, 49.8, 3735, '33030000', NULL, NULL),
(63, '102633000410', '0113800540', 90, 35.4, 3186, '33030000', NULL, NULL),
(65, '102059463330', '3101200069', 84, 39, 3276, '22082050', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `wh_exchangerates`
--

CREATE TABLE `wh_exchangerates` (
  `exchangerate_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `exchangerate_code` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `exchangerate_rate` double NOT NULL,
  `exchangerate_status` int(1) NOT NULL DEFAULT 1 COMMENT '1 : hiển thị, 0: ẩn',
  `exchangerate_order` int(10) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `wh_file_upload`
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
-- Dumping data for table `wh_file_upload`
--

INSERT INTO `wh_file_upload` (`file_id`, `file_type`, `file_path`, `user_id`, `file_created_at`, `file_name`) VALUES
(1, 1, 'Seal/1841312467_TemplateHangDieuChuyen1.xlsx', 3, '2020-08-31 03:15:18', '1841312467_TemplateHangDieuChuyen1.xlsx'),
(2, 1, 'Seal/1689768962_TemplateHangDieuChuyen1.xlsx', 3, '2020-08-31 03:20:59', '1689768962_TemplateHangDieuChuyen1.xlsx'),
(3, 1, 'Seal/1755433505_TemplateHangDieuChuyen.xlsx', 3, '2020-09-01 07:13:25', '1755433505_TemplateHangDieuChuyen.xlsx'),
(4, 1, 'Seal/952962531_TemplateHangDieuChuyen.xlsx', 3, '2020-09-01 11:27:20', '952962531_TemplateHangDieuChuyen.xlsx'),
(5, 2, 'Sell/804768068_TemplateDuLieuBanHang2.xlsx', 3, '2020-09-03 02:53:40', '804768068_TemplateDuLieuBanHang2.xlsx'),
(6, 1, 'Seal/1004079405_TemplateHangDieuChuyen.xlsx', 3, '2020-09-03 08:58:35', '1004079405_TemplateHangDieuChuyen.xlsx'),
(7, 2, 'Sell/1708747555_TemplateDuLieuBanHang.xlsx', 3, '2020-09-03 13:03:38', '1708747555_TemplateDuLieuBanHang.xlsx'),
(8, 1, 'Seal/781109947_TemplateHangDieuChuyen10.xlsx', 3, '2020-09-23 02:59:35', '781109947_TemplateHangDieuChuyen10.xlsx'),
(9, 2, 'Sell/1146444611_TemplateDuLieuBanHang12.xlsx', 3, '2020-09-23 03:20:13', '1146444611_TemplateDuLieuBanHang12.xlsx'),
(10, 1, 'Seal/113956889_TemplateHangDieuChuyen11.xlsx', 3, '2020-09-24 03:54:35', '113956889_TemplateHangDieuChuyen11.xlsx'),
(11, 2, 'Sell/1652893578_TemplateDuLieuBanHang15.xlsx', 3, '2020-09-24 03:57:51', '1652893578_TemplateDuLieuBanHang15.xlsx'),
(12, 2, 'Sell/485364560_TemplateDuLieuBanHang15.xlsx', 3, '2020-09-24 03:59:12', '485364560_TemplateDuLieuBanHang15.xlsx'),
(13, 1, 'Seal/2092509142_TemplateHangDieuChuyen12.xlsx', 3, '2020-09-24 04:08:42', '2092509142_TemplateHangDieuChuyen12.xlsx'),
(14, 1, 'Seal/349374748_TemplateHangDieuChuyen13.xlsx', 3, '2020-09-24 04:14:22', '349374748_TemplateHangDieuChuyen13.xlsx'),
(15, 2, 'Sell/768338717_TemplateDuLieuBanHang16.xlsx', 3, '2020-09-24 04:15:07', '768338717_TemplateDuLieuBanHang16.xlsx'),
(16, 1, 'Seal/933257581_TemplateHangDieuChuyen4.xlsx', 3, '2020-09-25 02:14:53', '933257581_TemplateHangDieuChuyen4.xlsx'),
(17, 2, 'Sell/399437125_TemplateDuLieuBanHang.xlsx', 3, '2020-09-25 02:17:58', '399437125_TemplateDuLieuBanHang.xlsx');

-- --------------------------------------------------------

--
-- Table structure for table `wh_inventory`
--

CREATE TABLE `wh_inventory` (
  `in_id` int(11) NOT NULL,
  `de_number` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `in_quantity` int(11) NOT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `settlement_date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `wh_inventory`
--

INSERT INTO `wh_inventory` (`in_id`, `de_number`, `in_quantity`, `product_code`, `settlement_date`) VALUES
(1, '102471574441', 20, '0153600039', NULL),
(2, '102471574441', 20, '0153600037', NULL),
(3, '102471574441', 31, '0113800502', NULL),
(4, '102471574441', 34, '0113800503', NULL),
(5, '102471591721', 14, '16F1100006', NULL),
(6, '102471591721', 9, '04D8000001', NULL),
(7, '102471591721', 3, '0454800059', '2020-09-03 17:06:15'),
(8, '102471591721', 23, '0454800055', NULL),
(9, '102471591721', 9, '0452100963', NULL),
(10, '102471591721', 19, '0454800035', NULL),
(11, '102471594740', 17, '15G0400001', NULL),
(12, '102471594740', 14, '1596500141', NULL),
(13, '102471594740', 5, '15D4500002', '2020-09-03 17:06:15'),
(14, '102471594740', 21, '15G0300001', NULL),
(15, '102471594740', 5, '15G6000002', '2020-09-03 17:06:15'),
(16, '102471601960', 19, '1655500083', NULL),
(17, '102471601960', 5, '16D2400017', '2020-09-03 17:06:15'),
(19, '102471574440', 0, '0153600039', '2020-09-23 10:47:39'),
(20, '102471574440', 0, '0153600037', '2020-09-23 10:47:39'),
(21, '102471574440', 0, '0113800503', '2020-09-23 10:47:39'),
(22, '102471574440', 0, '0113800502', '2020-09-23 10:47:39'),
(23, '102633000410', 71, '0153600045', NULL),
(24, '102633000410', 71, '0153600048', NULL),
(25, '102633000410', 71, '0141500255', NULL),
(26, '102633000410', 90, '0113800527', NULL),
(27, '102633000410', 91, '01D2200004', NULL),
(28, '102633000410', 30, '01E9000005', NULL),
(29, '102633000410', 30, '0129100004', NULL),
(30, '102633000410', 30, '0118100045', NULL),
(31, '102633000410', 70, '0186200001', NULL),
(32, '102633000410', 90, '0113800540', NULL),
(33, '102059463330', 74, '3101200069', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `wh_menus`
--

CREATE TABLE `wh_menus` (
  `menu_id` int(11) NOT NULL,
  `menu_name` varchar(255) NOT NULL,
  `menu_start_time` datetime NOT NULL,
  `menu_stop_time` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `wh_menus`
--

INSERT INTO `wh_menus` (`menu_id`, `menu_name`, `menu_start_time`, `menu_stop_time`) VALUES
(1, 'Default', '2020-09-01 15:21:30', '2020-09-01 15:21:30');

-- --------------------------------------------------------

--
-- Table structure for table `wh_menu_details`
--

CREATE TABLE `wh_menu_details` (
  `menu_detail_id` int(10) UNSIGNED NOT NULL,
  `menu_id` int(11) DEFAULT NULL,
  `product_code` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `menu_detail_parlever` int(11) DEFAULT NULL,
  `menu_detail_order` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `wh_menu_details`
--

INSERT INTO `wh_menu_details` (`menu_detail_id`, `menu_id`, `product_code`, `menu_detail_parlever`, `menu_detail_order`) VALUES
(23, 1, '0186200001', 5, 19),
(24, 1, '01D2200004', 3, 20),
(25, 1, '0153600045', 5, 21),
(26, 1, '0153600048', 5, 22),
(27, 1, '0141500255', 10, 23),
(63, 1, '0118100045', 5, 24),
(64, 1, '0129100004', 1, 25),
(65, 1, '3101200069', 1, 26),
(67, 1, '15E1300006', 1, 27),
(68, 1, '15E1300003', 1, 28),
(70, 1, '0242301549', 1, 29),
(71, 1, '01E9000005', 1, 30),
(72, 1, '0113800540', 1, 31),
(73, 1, '0113800527', 1, 32);

-- --------------------------------------------------------

--
-- Table structure for table `wh_modules`
--

CREATE TABLE `wh_modules` (
  `module_id` int(11) NOT NULL,
  `module_name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `module_created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `module_enable` bit(1) DEFAULT NULL,
  `module_code` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `wh_modules`
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
-- Table structure for table `wh_permissions`
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
-- Dumping data for table `wh_permissions`
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
-- Table structure for table `wh_products`
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
-- Dumping data for table `wh_products`
--

INSERT INTO `wh_products` (`product_code`, `product_name`, `product_unit`, `product_type`, `product_parlevel`, `product_status`, `product_createdat`) VALUES
('0113800502', 'Bộ nước hoa nữ hiệu Bvlgari (Bvlgari The Women\'s Gift Collection 5mlx5), 0113800502', 'Bộ', 'Nước hoa', 1, 1, '2020-08-27 07:59:05'),
('0113800503', 'Bộ nước hoa nam hiệu Bvlgari (Bvlgari The Men\'s Gift Collection 5mlx5), 0113800503', 'Bộ', 'Nước hoa', 1, 1, '2020-08-27 07:59:05'),
('0113800527', 'Bộ nước hoa nữ hiệu Bulgari (The Bulgari Women\'s Gift Collection, 5mlx5), 0113800527', 'SET', 'Nước Hoa', 1, 1, '2020-09-24 02:00:14'),
('0113800540', 'Bộ nước hoa nam hiệu Bulgari (The Bulgari Men\'s Gift Collection, 15mlx5), 0113800540', 'SET', 'Nước Hoa', 1, 1, '2020-09-24 02:00:32'),
('0118100045', 'Bộ nước hoa hiệu Dolce & Gabbana (Dolce & Gabbana Miniature Set, 4.5mlx2, 5mlx2, 7.5ml), 0118100045', 'SET', 'Nước Hoa', 1, 1, '2020-09-24 02:01:29'),
('0129100004', 'Nước hoa hiệu Acqua di Giò ( Acqua di Giò Pour Homme EDT 50ml Spray), 0129100004', 'UNA', 'Nước Hoa', 1, 1, '2020-09-24 02:01:10'),
('0141500255', 'Bộ nước hoa hiệu Lancome ( Lancome Best Of Fragrances boxed 5mlx3, 7.5ml, 4ml), 0141500255', 'SET', 'Nước Hoa', 1, 1, '2020-09-24 01:59:41'),
('0153600037', 'Nước hoa hiệu Paco Rabanne ( Paco Rabanne 1 Million EDT 50ml ), 0153600037', 'Cái', 'Nước hoa', 1, 1, '2020-09-01 07:55:05'),
('0153600039', 'Nước hoa nữ hiệu Paco Rabanne ( Paco Rabanne Lady Million EDP 50ml ), 0153600039', 'Cái', 'Nước hoa', 1, 1, '2020-09-01 07:55:05'),
('0153600045', 'Nước hoa hiệu Paco Rabanne (Paco Rabanne 1 Million EDT 100ml ), 0153600045', 'UNA', 'Nước Hoa', 1, 1, '2020-09-24 01:58:59'),
('0153600048', 'Nước hoa nữ hiệu Paco Rabanne (Paco Rabanne Lady Million EDP 100ml), 0153600048', 'UNA', 'Nước Hoa', 1, 1, '2020-09-24 01:59:21'),
('0186200001', 'Nước hoa CHLOÉ (CHLOÉ SIGNATURE EDP 50ML (ALCOHOL 75.9%)), 0186200001', 'UNA', 'Nước Hoa', 1, 1, '2020-09-24 01:55:22'),
('01D2200004', 'Bộ nước hoa Charrier Perfumes (Charrier Perfumes - AIR DE FRANCE Pack of 3 Miniatures EDP 15ml (Total)), 01D2200004', 'SET', 'Nước Hoa', 1, 1, '2020-09-24 01:54:38'),
('01E9000005', 'Chai xịt thơm Victoria\'s Secret (Victoria\'s Secret Fantasies Body Mist  250 ml- Pure Seduction), 01E9000005', 'UNA', 'Nước Hoa', 1, 1, '2020-09-24 02:00:51'),
('0225200378', 'Mỹ phẩm hiệu Elizabeth Arden (Bộ phấn mắt và son môi dưỡng ẩm Elizabeth Arden Eye & Lip Beauty Perfection, 5x5.67g eye shadow, 4x3.4g lipstick), xuất xứ China và Korea, 0225200378', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:03:58'),
('0225200379', 'Bộ kem dưỡng da tay trà xanh Elizabeth Arden (Elizabeth Arden Green Tea Hand Cream Set (4 x 30ml)), 0225200379', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:05:57'),
('0241502135', 'Mỹ phẩm hiệu Lancôme ( Tinh chất trẻ hóa da Lancôme Advanced Génifique Youth Activating Concentrate 50ml) , 0241502135', 'PCS', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:04:33'),
('0241502243', 'Mỹ phẩm hiệu Lancôme (Tinh chất trẻ hóa da Lancôme Advanced Génifque Yeux Light Pearl 20ml), 0241502243', 'UNA', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:04:55'),
('0241502264', 'Mỹ phẩm hiệu Lancôme (Hoạt chất dưỡng da Lancôme Advanced Génifique Sensitive 20 ml), 0241502264', 'UNA', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:05:19'),
('0242301548', 'Mỹ phẩm hiệu L\'Oreal (Bộ 3 cây son môi L\'Oreal CC Balm Genius by Balm Caresse - Colour Shades 3gx3), 0242301548', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:05:35'),
('0242301549', 'Mỹ phẩm hiệu L\'Oreal (Bộ 3 cây son môi L\'Oreal Look On The Go - Parisian Glamour, 40g x 3), 0242301549', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:03:15'),
('0253300364', 'Mỹ phẩm hiệu L\'Occitane (Bộ kem dưỡng da tay L\'Occitane Sweet Hands Kit, 6x30ml), 0253300364', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:03:33'),
('0253300365', 'Mỹ phẩm hiệu L\'Occitane (Bộ 3 son dưỡng L\'Occitane Shea & Honey Lip Balm Trio, 3x12ml), 0253300365', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:06:14'),
('02G4200001', 'Bộ dầu dưỡng tóc hiệu Moroccanoil (Moroccanoil 50ml+25m Travller Set (individual packed), 02G4200001', 'SET', 'Mỹ Phẩm', 1, 1, '2020-09-24 02:06:36'),
('0452100963', 'Vòng cổ \"Coral\" hiệu Pica Lela kèm hoa tai (Pica LéLa Australia \"Coral\" 18K Rose Gold Plated Necklace WithCats Eye Centre Stone), 0452100963', 'Cái', 'Trang sức', 1, 1, '2020-09-01 07:55:05'),
('0452101052', 'Vòng cổ \"Sweet Duchess\" hiệu Pica Lela kèm hoa tai (Pica LéLa \"Sweet Duchess\" 18K Rose Gold Plated Necklace), 0452101052', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:41:43'),
('0454800035', 'Bộ vòng tay hiệu Pierre Cardin ( Pierre Cardin Bangle Set ), 0454800035', 'Bộ', 'Trang sức', 1, 1, '2020-09-01 07:55:05'),
('0454800055', 'Bộ 9 đôi hoa tai hiệu Pierre Cardin (Pierre Cardin Earring Set), 0454800055', 'Bộ', 'Trang sức', 1, 1, '2020-09-01 07:55:05'),
('0454800059', 'Bộ hoa tai ngọc trai hiệu Pierre Cardin (Pierre Cardin Pearl Earring Set), 0454800059', 'Bộ', 'Trang sức', 1, 1, '2020-09-01 07:55:06'),
('0497300048', 'Bộ 8 đôi hoa tai hiệu Buckley London (Buckley London Rose Gold Earrings Set of 8), 0497300048', 'SET', 'Phụ Kiện', 1, 1, '2020-09-24 02:25:29'),
('0497300050', 'Bộ 8 đôi hoa tai hiệu Buckley London (Buckley London Rhodium Earring Set of 8), 0497300050', 'SET', 'Phụ Kiện', 1, 1, '2020-09-24 02:26:46'),
('04D8000001', 'Vòng đeo tay kèm 1 đôi bông tai hiệu Joia  ( Joia De Majorca Pearl Bracelet With Free Earrings ), 04D8000001', 'Cái', 'Trang sức', 1, 1, '2020-09-01 07:55:06'),
('04D8500008', 'Bộ đồng hồ kèm 2 vòng tay Fierro (Fierro Allure Watch and Double Bangle Set), 04D8500008', 'SET', 'Đồng Hồ', 1, 1, '2020-09-24 02:55:10'),
('04G1100006', 'Vòng tay hiệu Infinity & Co (Love X Infinity Heart Moon and Star Bracelet), 04G1100006', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:27:41'),
('04G1100007', 'Mặt dây chuyền hiệu Infinity & Co (Love X Infinity Heart Moon and Star Pendant), 04G1100007', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:28:37'),
('06E9000001', 'Bộ ba túi hiệu Victoria\'s Secret (Victoria\'s Secret Signature Trio Bag, 34 x 39 x 11 cm), 06E9000001', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:40:13'),
('06E9000004', 'Túi đựng mỹ phẩm hiệu Victoria\'s Secret (Victoria\'s Secret Signature Medium Cosmetics Bag, 15 x 25 x 6 cm), 06E9000004', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:40:44'),
('06E9000005', 'Túi đeo chéo hiệu Victoria\'s Secret (Victoria\'s Secret Supermodel Nylon CrossbodyNylon Bag), 06E9000005', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:41:05'),
('113800502', 'Bộ nước hoa nữ hiệu Bvlgari (Bvlgari The Women\'s Gift Collection 5mlx5), 0113800502', 'Bộ', 'Nước hoa', 1, 0, '2020-09-01 07:55:05'),
('113800503', 'Bộ nước hoa nam hiệu Bvlgari (Bvlgari The Men\'s Gift Collection 5mlx5), 0113800503', 'Bộ', 'Nước hoa', 1, 0, '2020-09-01 07:55:05'),
('1548700019', 'Bộ 2 đồng hồ 3 kim nam và nữ hiệu Beverly Hills Polo Club ( Beverly Hills Polo Club Gents and Ladies Matching Watch Set ), dây da nhân tạo, 1548700019', 'SET', 'Đồng Hồ', 1, 1, '2020-09-24 02:54:51'),
('1548700023', 'Bộ 2 đồng hồ nam và nữ hiệu Beverly Hills Polo Club ( Beverly Hills Polo Club Ladies and Gents Watch Set ), dây da nhân tạo, 1548700023', 'SET', 'Đồng hồ', 1, 1, '2020-09-24 02:49:02'),
('1596500114', 'Bộ đồng hồ nam và nữ hiệu Sekonda (Sekonda Gents & Ladies Watch Set), 1596500114', 'SET', 'Đồng Hồ', 1, 1, '2020-09-24 02:55:31'),
('1596500141', 'Đồng hồ nam hiệu Sekonda ( Sekonda Gents Sports Watch ), 1596500141', 'Cái', 'Đồng hồ', 1, 1, '2020-09-01 07:55:06'),
('15D2700002', 'Đồng hồ hiệu Fervor (Fervor Elegante Watch And Jewellery Set (Gold)) kèm bộ trang sức (dây chuyền, 2 đôi hoa tai, nhẫn), 15D2700002', 'SET', 'Đồng Hồ', 1, 1, '2020-09-24 02:55:53'),
('15D4500002', 'Đồng hồ nam hiệu Kenneth Cole (Kenneth Cole \"Automatic\" Gents Watch), 15D4500002', 'Cái', 'Đồng hồ', 1, 1, '2020-09-01 07:55:06'),
('15E1300003', 'Đồng hồ hiệu Tres Jolie ( Tres Jolie Rose Gold Dress Watch, dây thép không gỉ, 3 kim - tặng kèm 1 đôi bông tai ), 15E1300003', 'SET', 'Đồng Hồ', 1, 1, '2020-09-24 02:56:33'),
('15E1300006', 'Đồng hồ hiệu Tres Jolie (Tres Jolie Stardust Ladies Watch, dây da, 3 kim), 15E1300006', 'PCS', 'Đồng Hồ', 1, 1, '2020-09-24 02:56:53'),
('15G0300001', 'Đồng hồ nam hiệu Geoffrey Beene (Geoffrey Beene Manhattan Collection Gents Watch) kèm dây da, 15G0300001', 'Cái', 'Đồng hồ', 1, 1, '2020-09-01 07:55:06'),
('15G0400001', 'Đồng hồ nữ kèm vòng tay hiệu Ellen Tracy (Ellen Tracy Elegance Collection Ladies Gold Tone Watch), 15G0400001', 'Cái', 'Đồng hồ', 1, 1, '2020-09-01 07:55:06'),
('15G6000002', 'Bộ đồng hồ kèm 2 vòng tay hiệu Cristian Cole (Cristian Cole Glittering Watch & Bangles Set), 15G6000002', 'Bộ', 'Đồng hồ', 1, 1, '2020-09-01 07:55:06'),
('15G6600001', 'Đồng hồ nữ hiệu Tick & Ogle (Tick & Ogle Rose Delight White Leather Ladies Watch), 15G6600001', 'PCS', 'Đồng Hồ', 1, 1, '2020-09-24 02:50:40'),
('15G6600003', 'Đồng hồ nam hiệu Tick & Ogle (Tick & Ogle Herman Leather Gents Rose Gold Watch), 15G6600003', 'PCS', 'Đồng Hồ', 1, 1, '2020-09-24 02:51:07'),
('1655500083', 'Camera kỹ thuật số dạng mắt kinh hiệu Akita ( Akita iCapture 2), 1655500083', 'Cái', 'Phụ kiện', 1, 1, '2020-09-01 07:55:06'),
('16D2400017', 'Thiết bị massage da mặt, mắt, da đầu hiệu Lifetrons Beauty (Lifetrons Beauty Essentials Facial Massage Kit), 3.7V, 50Hz, 16D2400017', 'Cái', 'Phụ kiện', 1, 1, '2020-09-01 07:55:06'),
('16E8400001', 'Tai nghe không dây hiệu Breo (Breo Wireless Earphones), 16E8400001', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:45:08'),
('16F1100006', 'Gậy \"tự sướng\" Click Stick (Thumbs Up! Click Stick), hiệu Thumbs Up, bằng nhôm và nhựa, 16F1100006', 'Cái', 'Phụ kiện', 1, 1, '2020-09-01 07:55:06'),
('16G6900001', 'Thiết bị đeo theo dõi sức khỏe hiệu Oaxis (Oaxis Omini Band+,kèm 3 dây), 16G6900001', 'PCS', 'Phụ Kiện', 1, 1, '2020-09-24 02:44:07'),
('16G7400001', 'Cáp sạc hiệu Allroundo (Allroundo The All-In-One Charging Cable), 5V/1.5A, kèm 3 đầu nối, 16G7400001', 'SET', 'Phụ Kiện', 1, 1, '2020-09-24 02:27:15'),
('3101200069', 'Rượu CHIVAS REGAL: Chivas Regal 18 Year Old Blended Scotch Whisky 75cl (40%), 3101200069', 'Chai', 'Rượu', 1, 1, '2020-09-24 04:28:22');

-- --------------------------------------------------------

--
-- Table structure for table `wh_seal`
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
-- Dumping data for table `wh_seal`
--

INSERT INTO `wh_seal` (`se_number`, `se_flightnumber`, `se_flightdate`, `se_acreg`, `se_status`, `se_export_date`, `se_return`, `se_import_date`, `se_citypair_id`, `se_route`) VALUES
('07092020', 'VJ808, VJ809', '2020-09-07 00:00:00', NULL, 2, '2020-09-07 23:05:17', '07092020', '2020-09-07 23:16:37', 2, 'HKT-SGN, SGN-HKT'),
('179', 'VJ850', '2019-01-31 00:00:00', 'VNA696', 2, '2020-09-01 14:37:44', NULL, '2020-09-01 14:47:02', NULL, NULL),
('180', 'VJ851', '2019-01-31 00:00:00', 'VNA696', 2, '2020-09-01 14:37:50', NULL, '2020-09-01 16:28:14', NULL, NULL),
('182', 'VJ811', '2019-01-31 00:00:00', 'VNA658', 2, '2020-09-01 14:37:54', NULL, '2020-09-01 16:28:22', NULL, NULL),
('183', 'VJ812', '2019-01-31 00:00:00', 'VNA658', 2, '2020-09-01 14:37:58', NULL, '2020-09-01 16:28:32', NULL, NULL),
('184', 'VJ813', '2019-01-31 00:00:00', 'VNA655', 2, '2020-09-03 09:48:15', NULL, '2020-09-03 09:53:59', NULL, NULL),
('185', 'VJ852', '2019-01-31 00:00:00', 'VNA622', 2, '2020-09-03 09:49:38', NULL, '2020-09-03 09:54:08', NULL, NULL),
('186', 'VJ811, VJ812', '2019-01-02 00:00:00', 'VNA655', 2, '2020-09-03 17:05:24', NULL, '2020-09-03 17:36:14', NULL, NULL),
('187', 'VJ822, VJ823', '2019-01-02 00:00:00', 'VNA622', 2, '2020-09-03 17:05:31', NULL, '2020-09-05 09:44:49', NULL, NULL),
('188', 'VJ840, VJ841', '2019-01-02 00:00:00', 'VNA526', 2, '2020-09-03 17:05:40', NULL, '2020-09-05 09:45:00', NULL, NULL),
('189', 'VJ850, VJ851', '2019-01-02 00:00:00', 'VNA877', 2, '2020-09-03 17:05:53', NULL, '2020-09-05 09:45:08', NULL, NULL),
('190', 'VJ855, VJ886', '2019-01-02 00:00:00', 'VNA985', 2, '2020-09-03 17:06:03', NULL, '2020-09-05 09:45:16', NULL, NULL),
('191', 'VJ860, VJ861', '2019-01-02 00:00:00', 'VNA885', 2, '2020-09-03 17:06:15', NULL, '2020-09-05 09:45:27', NULL, NULL),
('569292020', 'VJ202', '2020-09-23 00:00:00', 'VNA202', 2, '2020-09-23 10:18:16', NULL, '2020-09-23 10:29:39', NULL, NULL),
('569298222', 'VJ222', '2020-08-31 00:00:00', 'VNA222', 2, '2020-08-31 10:17:54', NULL, '2020-09-01 14:22:13', NULL, NULL),
('569298223', 'VJ223', '2020-08-31 00:00:00', 'VNA223', 2, '2020-08-31 10:25:13', NULL, '2020-08-31 10:25:44', NULL, NULL),
('569298249', 'VJ124', '2020-09-24 00:00:00', 'VNA249', 2, '2020-09-24 10:55:33', NULL, '2020-09-24 11:00:16', NULL, NULL),
('569298429', 'VJ124', '2020-09-24 00:00:00', 'VNA699', 2, '2020-09-24 11:09:02', NULL, '2020-09-24 11:19:26', NULL, NULL),
('569298888', 'VJ259', '2020-09-25 00:00:00', 'VNA699', 2, '2020-09-25 09:15:14', NULL, '2020-09-25 09:37:29', NULL, NULL),
('569298924', 'VJ924', '2020-09-24 00:00:00', 'VNA699', 2, '2020-09-24 11:14:45', NULL, '2020-09-24 11:18:24', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `wh_seal_detail`
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
-- Dumping data for table `wh_seal_detail`
--

INSERT INTO `wh_seal_detail` (`sealdetail_id`, `seal_number`, `de_number`, `product_code`, `sealdetail_quantity_sell`, `sealdetail_quantity_export`, `sealdetail_quantity_inventory`, `sealdetail_quantity_real`) VALUES
(1, '184', '102471591721', '0454800059', 0, 1, 1, 1),
(2, '184', '102471574441', '0153600039', 0, 1, 1, 1),
(3, '184', '102471574441', '0153600037', 0, 1, 1, 1),
(4, '184', '102471574441', '0113800503', 1, 1, 0, 0),
(5, '184', '102471574441', '0113800502', 1, 1, 0, 0),
(6, '184', '102471591721', '16F1100006', 0, 1, 1, 1),
(7, '184', '102471601960', '1655500083', 0, 1, 1, 1),
(8, '184', '102471591721', '0452100963', 0, 1, 1, 1),
(9, '184', '102471594740', '15G6000002', 0, 1, 1, 1),
(10, '184', '102471594740', '15G0300001', 0, 1, 1, 1),
(11, '184', '102471594740', '15D4500002', 0, 1, 1, 1),
(12, '184', '102471594740', '1596500141', 0, 1, 1, 1),
(13, '184', '102471591721', '04D8000001', 0, 1, 1, 1),
(14, '184', '102471591721', '0454800035', 0, 1, 1, 1),
(15, '184', '102471591721', '0454800055', 0, 1, 1, 1),
(16, '184', '102471594740', '15G0400001', 0, 1, 1, 1),
(17, '184', '102471601960', '16D2400017', 0, 1, 1, 1),
(18, '185', '102471591721', '0452100963', 0, 1, 1, 1),
(19, '185', '102471591721', '0454800059', 0, 1, 1, 1),
(20, '185', '102471591721', '04D8000001', 0, 1, 1, 1),
(21, '185', '102471594740', '1596500141', 0, 1, 1, 1),
(22, '185', '102471594740', '15D4500002', 0, 1, 1, 1),
(23, '185', '102471594740', '15G0300001', 0, 1, 1, 1),
(24, '185', '102471594740', '15G0400001', 0, 1, 1, 1),
(25, '185', '102471591721', '0454800035', 0, 1, 1, 1),
(26, '185', '102471594740', '15G6000002', 0, 1, 1, 1),
(27, '185', '102471601960', '16D2400017', 0, 1, 1, 1),
(28, '185', '102471591721', '16F1100006', 0, 1, 1, 1),
(29, '185', '102471574441', '0113800502', 0, 1, 1, 1),
(30, '185', '102471574441', '0113800503', 0, 1, 1, 1),
(31, '185', '102471574441', '0153600037', 0, 1, 1, 1),
(32, '185', '102471574441', '0153600039', 1, 1, 0, 0),
(33, '185', '102471601960', '1655500083', 0, 1, 1, 1),
(34, '185', '102471591721', '0454800055', 0, 1, 1, 1),
(35, '186', '102471591721', '0454800059', 0, 1, 1, 1),
(36, '186', '102471594740', '1596500141', 0, 1, 1, 1),
(37, '186', '102471594740', '15D4500002', 0, 1, 1, 1),
(38, '186', '102471594740', '15G0300001', 0, 1, 1, 1),
(39, '186', '102471594740', '15G0400001', 0, 1, 1, 1),
(40, '186', '102471594740', '15G6000002', 0, 1, 1, 1),
(41, '186', '102471601960', '1655500083', 0, 1, 1, 1),
(42, '186', '102471591721', '04D8000001', 0, 1, 1, 1),
(43, '186', '102471601960', '16D2400017', 0, 1, 1, 1),
(44, '186', '102471574441', '0113800502', 0, 1, 1, 1),
(45, '186', '102471574441', '0153600037', 0, 1, 1, 1),
(46, '186', '102471574441', '0153600039', 0, 1, 1, 1),
(47, '186', '102471591721', '0452100963', 0, 1, 1, 1),
(48, '186', '102471591721', '0454800035', 0, 1, 1, 1),
(49, '186', '102471591721', '0454800055', 0, 1, 1, 1),
(50, '186', '102471591721', '16F1100006', 0, 1, 1, 1),
(51, '186', '102471574441', '0113800503', 0, 1, 1, 1),
(52, '187', '102471594740', '15G0400001', 0, 1, 1, 1),
(53, '187', '102471574441', '0153600037', 1, 1, 0, 0),
(54, '187', '102471574441', '0153600039', 1, 1, 0, 0),
(55, '187', '102471591721', '0452100963', 0, 1, 1, 1),
(56, '187', '102471591721', '0454800035', 0, 1, 1, 1),
(57, '187', '102471591721', '0454800055', 0, 1, 1, 1),
(58, '187', '102471601960', '16D2400017', 0, 1, 1, 1),
(59, '187', '102471574441', '0113800503', 0, 1, 1, 1),
(60, '187', '102471591721', '16F1100006', 0, 1, 1, 1),
(61, '187', '102471594740', '15G6000002', 0, 1, 1, 1),
(62, '187', '102471591721', '0454800059', 0, 1, 1, 1),
(63, '187', '102471591721', '04D8000001', 0, 1, 1, 1),
(64, '187', '102471594740', '1596500141', 0, 1, 1, 1),
(65, '187', '102471594740', '15D4500002', 0, 1, 1, 1),
(66, '187', '102471594740', '15G0300001', 0, 1, 1, 1),
(67, '187', '102471601960', '1655500083', 0, 1, 1, 1),
(68, '187', '102471574441', '0113800502', 0, 1, 1, 1),
(69, '188', '102471591721', '0454800055', 0, 1, 1, 1),
(70, '188', '102471591721', '04D8000001', 0, 1, 1, 1),
(71, '188', '102471594740', '1596500141', 0, 1, 1, 1),
(72, '188', '102471594740', '15G0300001', 0, 1, 1, 1),
(73, '188', '102471594740', '15G0400001', 0, 1, 1, 1),
(74, '188', '102471594740', '15G6000002', 0, 1, 1, 1),
(75, '188', '102471601960', '1655500083', 0, 1, 1, 1),
(76, '188', '102471591721', '0454800059', 0, 1, 1, 1),
(77, '188', '102471601960', '16D2400017', 0, 1, 1, 1),
(78, '188', '102471574441', '0113800502', 0, 1, 1, 1),
(79, '188', '102471574441', '0113800503', 0, 1, 1, 1),
(80, '188', '102471574441', '0153600037', 0, 1, 1, 1),
(81, '188', '102471574441', '0153600039', 0, 1, 1, 1),
(82, '188', '102471591721', '0452100963', 1, 1, 0, 0),
(83, '188', '102471591721', '0454800035', 1, 1, 0, 0),
(84, '188', '102471591721', '16F1100006', 0, 1, 1, 1),
(85, '188', '102471594740', '15D4500002', 0, 1, 1, 1),
(86, '189', '102471591721', '0454800055', 1, 1, 0, 0),
(87, '189', '102471594740', '1596500141', 0, 1, 1, 1),
(88, '189', '102471594740', '15D4500002', 0, 1, 1, 1),
(89, '189', '102471594740', '15G0300001', 0, 1, 1, 1),
(90, '189', '102471594740', '15G0400001', 0, 1, 1, 1),
(91, '189', '102471594740', '15G6000002', 0, 1, 1, 1),
(92, '189', '102471601960', '1655500083', 0, 1, 1, 1),
(93, '189', '102471591721', '04D8000001', 0, 1, 1, 1),
(94, '189', '102471601960', '16D2400017', 0, 1, 1, 1),
(95, '189', '102471574441', '0113800502', 0, 1, 1, 1),
(96, '189', '102471574441', '0113800503', 0, 1, 1, 1),
(97, '189', '102471574441', '0153600037', 0, 1, 1, 1),
(98, '189', '102471574441', '0153600039', 0, 1, 1, 1),
(99, '189', '102471591721', '0452100963', 0, 1, 1, 1),
(100, '189', '102471591721', '0454800035', 0, 1, 1, 1),
(101, '189', '102471591721', '16F1100006', 0, 1, 1, 1),
(102, '189', '102471591721', '0454800059', 1, 1, 0, 0),
(103, '190', '102471594740', '15G0400001', 0, 1, 1, 1),
(104, '190', '102471574441', '0153600037', 0, 1, 1, 1),
(105, '190', '102471574441', '0153600039', 0, 1, 1, 1),
(106, '190', '102471591721', '0452100963', 0, 1, 1, 1),
(107, '190', '102471591721', '0454800035', 0, 1, 1, 1),
(108, '190', '102471591721', '0454800055', 0, 1, 1, 1),
(109, '190', '102471591721', '16F1100006', 1, 1, 0, 0),
(110, '190', '102471574441', '0113800503', 0, 1, 1, 1),
(111, '190', '102471601960', '16D2400017', 0, 1, 1, 1),
(112, '190', '102471594740', '15G6000002', 0, 1, 1, 1),
(113, '190', '102471591721', '0454800059', 0, 1, 1, 1),
(114, '190', '102471591721', '04D8000001', 1, 1, 0, 0),
(115, '190', '102471594740', '1596500141', 0, 1, 1, 1),
(116, '190', '102471594740', '15D4500002', 0, 1, 1, 1),
(117, '190', '102471594740', '15G0300001', 0, 1, 1, 1),
(118, '190', '102471601960', '1655500083', 0, 1, 1, 1),
(119, '190', '102471574441', '0113800502', 0, 1, 1, 1),
(120, '191', '102471591721', '0452100963', 0, 1, 1, 1),
(121, '191', '102471594740', '15D4500002', 1, 1, 0, 0),
(122, '191', '102471591721', '0454800059', 0, 0, 0, 0),
(123, '191', '102471591721', '04D8000001', 0, 1, 1, 1),
(124, '191', '102471594740', '1596500141', 1, 1, 0, 0),
(125, '191', '102471594740', '15G0300001', 1, 1, 0, 0),
(126, '191', '102471594740', '15G0400001', 1, 1, 0, 0),
(127, '191', '102471591721', '0454800035', 0, 1, 1, 1),
(128, '191', '102471594740', '15G6000002', 1, 1, 0, 0),
(129, '191', '102471601960', '16D2400017', 1, 1, 0, 0),
(130, '191', '102471591721', '16F1100006', 0, 1, 1, 1),
(131, '191', '102471574441', '0113800502', 0, 1, 1, 1),
(132, '191', '102471574441', '0113800503', 0, 1, 1, 1),
(133, '191', '102471574441', '0153600037', 0, 1, 1, 1),
(134, '191', '102471574441', '0153600039', 0, 1, 1, 1),
(135, '191', '102471601960', '1655500083', 1, 1, 0, 0),
(136, '191', '102471591721', '0454800055', 0, 1, 1, 1),
(137, '7654321', '102471591721', '0454800059', 0, 1, 1, 1),
(138, '7654321', '102471591721', '04D8000001', 0, 1, 1, 1),
(139, '7654321', '102471594740', '1596500141', 0, 1, 1, 1),
(140, '7654321', '102471594740', '15D4500002', 0, 1, 1, 1),
(141, '7654321', '102471594740', '15G0300001', 0, 1, 1, 1),
(142, '7654321', '102471594740', '15G0400001', 0, 1, 1, 1),
(143, '7654321', '102471594740', '15G6000002', 0, 1, 1, 1),
(144, '7654321', '102471601960', '1655500083', 0, 1, 1, 1),
(145, '7654321', '102471601960', '16D2400017', 0, 1, 1, 1),
(146, '7654321', '102471591721', '16F1100006', 0, 1, 1, 1),
(147, '7654321', '102471574441', '0113800502', 0, 1, 1, 1),
(148, '7654321', '102471574441', '0113800503', 0, 1, 1, 1),
(149, '7654321', '102471574441', '0153600037', 0, 1, 1, 1),
(150, '7654321', '102471574441', '0153600039', 0, 1, 1, 1),
(151, '7654321', '102471591721', '0452100963', 0, 1, 1, 1),
(152, '7654321', '102471591721', '0454800035', 0, 1, 1, 1),
(153, '7654321', '102471591721', '0454800055', 0, 1, 1, 1),
(154, '07092020', '102471591721', '0454800059', 0, 1, 1, 1),
(155, '07092020', '102471591721', '04D8000001', 0, 1, 1, 1),
(156, '07092020', '102471594740', '1596500141', 0, 1, 1, 1),
(157, '07092020', '102471594740', '15D4500002', 0, 1, 1, 1),
(158, '07092020', '102471594740', '15G0300001', 0, 1, 1, 1),
(159, '07092020', '102471594740', '15G0400001', 0, 1, 1, 1),
(160, '07092020', '102471594740', '15G6000002', 0, 1, 1, 1),
(161, '07092020', '102471601960', '1655500083', 0, 1, 1, 1),
(162, '07092020', '102471601960', '16D2400017', 0, 1, 1, 1),
(163, '07092020', '102471591721', '16F1100006', 0, 1, 1, 1),
(164, '07092020', '102471574441', '0113800502', 0, 1, 1, 1),
(165, '07092020', '102471574441', '0113800503', 0, 1, 1, 1),
(166, '07092020', '102471574441', '0153600037', 0, 1, 1, 1),
(167, '07092020', '102471574441', '0153600039', 0, 1, 1, 1),
(168, '07092020', '102471591721', '0452100963', 0, 1, 1, 1),
(169, '07092020', '102471591721', '0454800035', 0, 1, 1, 1),
(170, '07092020', '102471591721', '0454800055', 0, 1, 1, 1),
(171, '07072020', '102471591721', '0454800059', 1, 1, 0, 0),
(172, '07072020', '102471591721', '04D8000001', 0, 1, 1, 1),
(173, '07072020', '102471594740', '1596500141', 0, 1, 1, 1),
(174, '07072020', '102471594740', '15D4500002', 0, 1, 1, 1),
(175, '07072020', '102471594740', '15G0300001', 0, 1, 1, 1),
(176, '07072020', '102471594740', '15G0400001', 0, 1, 1, 1),
(177, '07072020', '102471594740', '15G6000002', 0, 1, 1, 1),
(178, '07072020', '102471601960', '1655500083', 0, 1, 1, 1),
(179, '07072020', '102471601960', '16D2400017', 0, 1, 1, 1),
(180, '07072020', '102471591721', '16F1100006', 0, 1, 1, 1),
(181, '07072020', '102471574441', '0113800502', 0, 1, 1, 1),
(182, '07072020', '102471574441', '0113800503', 1, 1, 0, 0),
(183, '07072020', '102471574441', '0153600037', 0, 1, 1, 1),
(184, '07072020', '102471574441', '0153600039', 0, 1, 1, 1),
(185, '07072020', '102471591721', '0452100963', 0, 1, 1, 1),
(186, '07072020', '102471591721', '0454800035', 0, 1, 1, 1),
(187, '07072020', '102471591721', '0454800055', 0, 1, 1, 1),
(188, '569292020', '102471591721', '0454800059', 0, 1, 1, 1),
(189, '569292020', '102471591721', '04D8000001', 0, 1, 1, 1),
(190, '569292020', '102471594740', '1596500141', 0, 1, 1, 1),
(191, '569292020', '102471594740', '15D4500002', 0, 1, 1, 1),
(192, '569292020', '102471594740', '15G0300001', 0, 1, 1, 1),
(193, '569292020', '102471594740', '15G0400001', 0, 1, 1, 1),
(194, '569292020', '102471594740', '15G6000002', 0, 1, 1, 1),
(195, '569292020', '102471601960', '1655500083', 0, 1, 1, 1),
(196, '569292020', '102471601960', '16D2400017', 0, 1, 1, 1),
(197, '569292020', '102471591721', '16F1100006', 0, 1, 1, 1),
(198, '569292020', '102471574441', '0113800502', 1, 1, 0, 0),
(199, '569292020', '102471574441', '0113800503', 1, 1, 0, 0),
(200, '569292020', '102471574441', '0153600037', 1, 1, 0, 0),
(201, '569292020', '102471574441', '0153600039', 0, 1, 1, 1),
(202, '569292020', '102471591721', '0452100963', 0, 1, 1, 1),
(203, '569292020', '102471591721', '0454800035', 0, 1, 1, 1),
(204, '569292020', '102471591721', '0454800055', 0, 1, 1, 1),
(205, '569298249', '102633000410', '0186200001', 4, 5, 1, 1),
(206, '569298249', '102633000410', '01D2200004', 2, 3, 1, 1),
(207, '569298249', '102633000410', '0153600045', 4, 5, 1, 1),
(208, '569298249', '102633000410', '0153600048', 4, 5, 1, 1),
(209, '569298249', '102633000410', '0141500255', 9, 10, 1, 1),
(210, '569298429', '102633000410', '0186200001', 0, 5, 5, 5),
(211, '569298429', '102633000410', '01D2200004', 0, 3, 3, 3),
(212, '569298429', '102633000410', '0153600045', 0, 5, 5, 5),
(213, '569298429', '102633000410', '0153600048', 0, 5, 5, 5),
(214, '569298429', '102633000410', '0141500255', 0, 10, 10, 10),
(215, '569298429', '102633000410', '0118100045', 0, 1, 1, 1),
(216, '569298429', '102633000410', '0129100004', 0, 1, 1, 1),
(217, '569298924', '102633000410', '0186200001', 0, 5, 5, 5),
(218, '569298924', '102633000410', '01D2200004', 0, 3, 3, 3),
(219, '569298924', '102633000410', '0153600045', 0, 5, 5, 5),
(220, '569298924', '102633000410', '0153600048', 0, 5, 5, 5),
(221, '569298924', '102633000410', '0141500255', 0, 10, 10, 10),
(222, '569298924', '102633000410', '0118100045', 5, 5, 0, 0),
(223, '569298924', '102633000410', '0129100004', 0, 1, 1, 1),
(224, '569298888', '102633000410', '0186200001', 1, 5, 4, 4),
(225, '569298888', '102633000410', '01D2200004', 0, 3, 3, 3),
(226, '569298888', '102633000410', '0153600045', 0, 5, 5, 5),
(227, '569298888', '102633000410', '0153600048', 0, 5, 5, 5),
(228, '569298888', '102633000410', '0141500255', 0, 10, 10, 10),
(229, '569298888', '102633000410', '0118100045', 0, 5, 5, 5),
(230, '569298888', '102633000410', '0129100004', 0, 1, 1, 1),
(231, '569298888', '102059463330', '3101200069', 0, 1, 1, 1);

-- --------------------------------------------------------

--
-- Table structure for table `wh_seal_product`
--

CREATE TABLE `wh_seal_product` (
  `id` bigint(20) NOT NULL,
  `seal_number` varchar(255) NOT NULL,
  `product_code` varchar(255) NOT NULL,
  `quantity_export` int(11) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `wh_seal_product`
--

INSERT INTO `wh_seal_product` (`id`, `seal_number`, `product_code`, `quantity_export`, `created_at`) VALUES
(1, '184', '0454800059', 1, '2020-09-01 11:27:20'),
(2, '185', '0452100963', 1, '2020-09-01 11:27:20'),
(3, '185', '0153600039', 1, '2020-09-01 11:27:20'),
(4, '185', '0153600037', 1, '2020-09-01 11:27:20'),
(5, '185', '0113800503', 1, '2020-09-01 11:27:20'),
(6, '185', '0113800502', 1, '2020-09-01 11:27:20'),
(7, '185', '16F1100006', 1, '2020-09-01 11:27:20'),
(8, '185', '16D2400017', 1, '2020-09-01 11:27:20'),
(9, '185', '1655500083', 1, '2020-09-01 11:27:20'),
(10, '185', '15G6000002', 1, '2020-09-01 11:27:20'),
(11, '185', '15G0400001', 1, '2020-09-01 11:27:20'),
(12, '185', '15G0300001', 1, '2020-09-01 11:27:20'),
(13, '185', '15D4500002', 1, '2020-09-01 11:27:20'),
(14, '185', '1596500141', 1, '2020-09-01 11:27:20'),
(15, '185', '04D8000001', 1, '2020-09-01 11:27:20'),
(16, '185', '0454800059', 1, '2020-09-01 11:27:20'),
(17, '184', '0454800055', 1, '2020-09-01 11:27:20'),
(18, '184', '0454800035', 1, '2020-09-01 11:27:20'),
(19, '184', '04D8000001', 1, '2020-09-01 11:27:20'),
(20, '184', '1596500141', 1, '2020-09-01 11:27:20'),
(21, '184', '15D4500002', 1, '2020-09-01 11:27:20'),
(22, '184', '15G0300001', 1, '2020-09-01 11:27:20'),
(23, '184', '15G0400001', 1, '2020-09-01 11:27:20'),
(24, '184', '15G6000002', 1, '2020-09-01 11:27:20'),
(25, '185', '0454800035', 1, '2020-09-01 11:27:20'),
(26, '184', '1655500083', 1, '2020-09-01 11:27:20'),
(27, '184', '16F1100006', 1, '2020-09-01 11:27:20'),
(28, '184', '0113800502', 1, '2020-09-01 11:27:20'),
(29, '184', '0113800503', 1, '2020-09-01 11:27:20'),
(30, '184', '0153600037', 1, '2020-09-01 11:27:20'),
(31, '184', '0153600039', 1, '2020-09-01 11:27:20'),
(32, '184', '0452100963', 1, '2020-09-01 11:27:20'),
(33, '184', '16D2400017', 1, '2020-09-01 11:27:20'),
(34, '185', '0454800055', 1, '2020-09-01 11:27:20'),
(35, '186', '0454800059', 1, '2020-09-03 08:58:35'),
(36, '190', '15G0400001', 1, '2020-09-03 08:58:35'),
(37, '190', '15G0300001', 1, '2020-09-03 08:58:35'),
(38, '190', '15D4500002', 1, '2020-09-03 08:58:35'),
(39, '190', '1596500141', 1, '2020-09-03 08:58:35'),
(40, '190', '04D8000001', 1, '2020-09-03 08:58:35'),
(41, '190', '0454800059', 1, '2020-09-03 08:58:35'),
(42, '189', '0454800055', 1, '2020-09-03 08:58:35'),
(43, '189', '0454800035', 1, '2020-09-03 08:58:35'),
(44, '189', '0452100963', 1, '2020-09-03 08:58:35'),
(45, '189', '0153600039', 1, '2020-09-03 08:58:35'),
(46, '189', '0153600037', 1, '2020-09-03 08:58:35'),
(47, '189', '0113800503', 1, '2020-09-03 08:58:35'),
(48, '189', '0113800502', 1, '2020-09-03 08:58:35'),
(49, '189', '16F1100006', 1, '2020-09-03 08:58:35'),
(50, '189', '16D2400017', 1, '2020-09-03 08:58:35'),
(51, '189', '1655500083', 1, '2020-09-03 08:58:35'),
(52, '189', '15G6000002', 1, '2020-09-03 08:58:35'),
(53, '189', '15G0400001', 1, '2020-09-03 08:58:35'),
(54, '189', '15G0300001', 1, '2020-09-03 08:58:35'),
(55, '189', '15D4500002', 1, '2020-09-03 08:58:35'),
(56, '189', '1596500141', 1, '2020-09-03 08:58:35'),
(57, '190', '15G6000002', 1, '2020-09-03 08:58:35'),
(58, '190', '1655500083', 1, '2020-09-03 08:58:35'),
(59, '190', '16D2400017', 1, '2020-09-03 08:58:35'),
(60, '190', '16F1100006', 1, '2020-09-03 08:58:35'),
(61, '191', '0452100963', 1, '2020-09-03 08:58:35'),
(62, '191', '0153600039', 1, '2020-09-03 08:58:35'),
(63, '191', '0153600037', 1, '2020-09-03 08:58:35'),
(64, '191', '0113800503', 1, '2020-09-03 08:58:35'),
(65, '191', '0113800502', 1, '2020-09-03 08:58:35'),
(66, '191', '16F1100006', 1, '2020-09-03 08:58:35'),
(67, '191', '16D2400017', 1, '2020-09-03 08:58:35'),
(68, '191', '1655500083', 1, '2020-09-03 08:58:35'),
(69, '191', '15G6000002', 1, '2020-09-03 08:58:35'),
(70, '191', '15G0400001', 1, '2020-09-03 08:58:35'),
(71, '189', '04D8000001', 1, '2020-09-03 08:58:35'),
(72, '191', '15G0300001', 1, '2020-09-03 08:58:35'),
(73, '191', '1596500141', 1, '2020-09-03 08:58:35'),
(74, '191', '04D8000001', 1, '2020-09-03 08:58:35'),
(75, '191', '0454800059', 1, '2020-09-03 08:58:35'),
(76, '190', '0454800055', 1, '2020-09-03 08:58:35'),
(77, '190', '0454800035', 1, '2020-09-03 08:58:35'),
(78, '190', '0452100963', 1, '2020-09-03 08:58:35'),
(79, '190', '0153600039', 1, '2020-09-03 08:58:35'),
(80, '190', '0153600037', 1, '2020-09-03 08:58:35'),
(81, '190', '0113800503', 1, '2020-09-03 08:58:35'),
(82, '190', '0113800502', 1, '2020-09-03 08:58:35'),
(83, '191', '15D4500002', 1, '2020-09-03 08:58:35'),
(84, '189', '0454800059', 1, '2020-09-03 08:58:35'),
(85, '188', '0454800055', 1, '2020-09-03 08:58:35'),
(86, '188', '0454800035', 1, '2020-09-03 08:58:35'),
(87, '187', '15G0400001', 1, '2020-09-03 08:58:35'),
(88, '187', '15G0300001', 1, '2020-09-03 08:58:35'),
(89, '187', '15D4500002', 1, '2020-09-03 08:58:35'),
(90, '187', '1596500141', 1, '2020-09-03 08:58:35'),
(91, '187', '04D8000001', 1, '2020-09-03 08:58:35'),
(92, '187', '0454800059', 1, '2020-09-03 08:58:35'),
(93, '186', '0454800055', 1, '2020-09-03 08:58:35'),
(94, '186', '0454800035', 1, '2020-09-03 08:58:35'),
(95, '186', '0452100963', 1, '2020-09-03 08:58:35'),
(96, '186', '0153600039', 1, '2020-09-03 08:58:35'),
(97, '187', '15G6000002', 1, '2020-09-03 08:58:35'),
(98, '186', '0153600037', 1, '2020-09-03 08:58:35'),
(99, '186', '0113800502', 1, '2020-09-03 08:58:35'),
(100, '186', '16F1100006', 1, '2020-09-03 08:58:35'),
(101, '186', '16D2400017', 1, '2020-09-03 08:58:35'),
(102, '186', '1655500083', 1, '2020-09-03 08:58:35'),
(103, '186', '15G6000002', 1, '2020-09-03 08:58:35'),
(104, '186', '15G0400001', 1, '2020-09-03 08:58:35'),
(105, '186', '15G0300001', 1, '2020-09-03 08:58:35'),
(106, '186', '15D4500002', 1, '2020-09-03 08:58:35'),
(107, '186', '1596500141', 1, '2020-09-03 08:58:35'),
(108, '186', '04D8000001', 1, '2020-09-03 08:58:35'),
(109, '186', '0113800503', 1, '2020-09-03 08:58:35'),
(110, '191', '0454800035', 1, '2020-09-03 08:58:35'),
(111, '187', '1655500083', 1, '2020-09-03 08:58:35'),
(112, '187', '16F1100006', 1, '2020-09-03 08:58:35'),
(113, '188', '0452100963', 1, '2020-09-03 08:58:35'),
(114, '188', '0153600039', 1, '2020-09-03 08:58:35'),
(115, '188', '0153600037', 1, '2020-09-03 08:58:35'),
(116, '188', '0113800503', 1, '2020-09-03 08:58:35'),
(117, '188', '0113800502', 1, '2020-09-03 08:58:35'),
(118, '188', '16F1100006', 1, '2020-09-03 08:58:35'),
(119, '188', '16D2400017', 1, '2020-09-03 08:58:35'),
(120, '188', '1655500083', 1, '2020-09-03 08:58:35'),
(121, '188', '15G6000002', 1, '2020-09-03 08:58:35'),
(122, '188', '15G0400001', 1, '2020-09-03 08:58:35'),
(123, '187', '16D2400017', 1, '2020-09-03 08:58:35'),
(124, '188', '15G0300001', 1, '2020-09-03 08:58:35'),
(125, '188', '1596500141', 1, '2020-09-03 08:58:35'),
(126, '188', '04D8000001', 1, '2020-09-03 08:58:35'),
(127, '188', '0454800059', 1, '2020-09-03 08:58:35'),
(128, '187', '0454800055', 1, '2020-09-03 08:58:35'),
(129, '187', '0454800035', 1, '2020-09-03 08:58:35'),
(130, '187', '0452100963', 1, '2020-09-03 08:58:35'),
(131, '187', '0153600039', 1, '2020-09-03 08:58:35'),
(132, '187', '0153600037', 1, '2020-09-03 08:58:35'),
(133, '187', '0113800503', 1, '2020-09-03 08:58:35'),
(134, '187', '0113800502', 1, '2020-09-03 08:58:35'),
(135, '188', '15D4500002', 1, '2020-09-03 08:58:35'),
(136, '191', '0454800055', 1, '2020-09-03 08:58:35'),
(154, '07092020', '0454800059', 1, '2020-09-07 16:02:01'),
(155, '07092020', '0452100963', 1, '2020-09-07 16:02:01'),
(156, '07092020', '0153600039', 1, '2020-09-07 16:02:01'),
(157, '07092020', '0153600037', 1, '2020-09-07 16:02:01'),
(158, '07092020', '0113800503', 1, '2020-09-07 16:02:01'),
(159, '07092020', '0113800502', 1, '2020-09-07 16:02:01'),
(160, '07092020', '16F1100006', 1, '2020-09-07 16:02:01'),
(161, '07092020', '0454800035', 1, '2020-09-07 16:02:01'),
(162, '07092020', '16D2400017', 1, '2020-09-07 16:02:01'),
(163, '07092020', '15G6000002', 1, '2020-09-07 16:02:01'),
(164, '07092020', '15G0400001', 1, '2020-09-07 16:02:01'),
(165, '07092020', '15G0300001', 1, '2020-09-07 16:02:01'),
(166, '07092020', '15D4500002', 1, '2020-09-07 16:02:01'),
(167, '07092020', '1596500141', 1, '2020-09-07 16:02:01'),
(168, '07092020', '04D8000001', 1, '2020-09-07 16:02:01'),
(169, '07092020', '1655500083', 1, '2020-09-07 16:02:01'),
(170, '07092020', '0454800055', 1, '2020-09-07 16:02:01'),
(188, '569292020', '0454800059', 1, '2020-09-23 02:59:34'),
(189, '569292020', '0452100963', 1, '2020-09-23 02:59:34'),
(190, '569292020', '0153600039', 1, '2020-09-23 02:59:34'),
(191, '569292020', '0153600037', 1, '2020-09-23 02:59:34'),
(192, '569292020', '0113800503', 1, '2020-09-23 02:59:34'),
(193, '569292020', '0113800502', 1, '2020-09-23 02:59:34'),
(194, '569292020', '16F1100006', 1, '2020-09-23 02:59:34'),
(195, '569292020', '0454800035', 1, '2020-09-23 02:59:34'),
(196, '569292020', '16D2400017', 1, '2020-09-23 02:59:34'),
(197, '569292020', '15G6000002', 1, '2020-09-23 02:59:34'),
(198, '569292020', '15G0400001', 1, '2020-09-23 02:59:34'),
(199, '569292020', '15G0300001', 1, '2020-09-23 02:59:34'),
(200, '569292020', '15D4500002', 1, '2020-09-23 02:59:34'),
(201, '569292020', '1596500141', 1, '2020-09-23 02:59:34'),
(202, '569292020', '04D8000001', 1, '2020-09-23 02:59:34'),
(203, '569292020', '1655500083', 1, '2020-09-23 02:59:34'),
(204, '569292020', '0454800055', 1, '2020-09-23 02:59:34'),
(205, '569298249', '0186200001', 5, '2020-09-24 03:54:35'),
(206, '569298249', '01D2200004', 3, '2020-09-24 03:54:35'),
(207, '569298249', '0153600045', 5, '2020-09-24 03:54:35'),
(208, '569298249', '0153600048', 5, '2020-09-24 03:54:35'),
(209, '569298249', '0141500255', 10, '2020-09-24 03:54:35'),
(210, '569298429', '0186200001', 5, '2020-09-24 04:08:42'),
(211, '569298429', '01D2200004', 3, '2020-09-24 04:08:42'),
(212, '569298429', '0153600045', 5, '2020-09-24 04:08:42'),
(213, '569298429', '0153600048', 5, '2020-09-24 04:08:42'),
(214, '569298429', '0141500255', 10, '2020-09-24 04:08:42'),
(215, '569298429', '0118100045', 1, '2020-09-24 04:08:42'),
(216, '569298429', '0129100004', 1, '2020-09-24 04:08:42'),
(217, '569298924', '0186200001', 5, '2020-09-24 04:14:22'),
(218, '569298924', '01D2200004', 3, '2020-09-24 04:14:22'),
(219, '569298924', '0153600045', 5, '2020-09-24 04:14:22'),
(220, '569298924', '0153600048', 5, '2020-09-24 04:14:22'),
(221, '569298924', '0141500255', 10, '2020-09-24 04:14:22'),
(222, '569298924', '0118100045', 5, '2020-09-24 04:14:22'),
(223, '569298924', '0129100004', 1, '2020-09-24 04:14:22'),
(224, '569298888', '0186200001', 5, '2020-09-25 02:14:53'),
(225, '569298888', '01D2200004', 3, '2020-09-25 02:14:53'),
(226, '569298888', '0153600045', 5, '2020-09-25 02:14:53'),
(227, '569298888', '0153600048', 5, '2020-09-25 02:14:53'),
(228, '569298888', '0141500255', 10, '2020-09-25 02:14:53'),
(229, '569298888', '0118100045', 5, '2020-09-25 02:14:53'),
(230, '569298888', '0129100004', 1, '2020-09-25 02:14:53'),
(231, '569298888', '3101200069', 1, '2020-09-25 02:14:53');

-- --------------------------------------------------------

--
-- Table structure for table `wh_sell`
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
-- Dumping data for table `wh_sell`
--

INSERT INTO `wh_sell` (`sl_id`, `sl_flight_no`, `sl_flight_date`, `sl_invoice_no`, `sl_customer_name`, `sl_passport_number`, `sl_nationality`, `sl_seat_number`, `sl_flight_number_detail`, `sl_type_invoice`) VALUES
(1599101620546, 'VJ813', '2019-01-31', '202041097', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', NULL, NULL),
(1599101620552, 'VJ813', '2019-01-31', '202041099', 'Nguyễn Văn B', 'A89051', 'VN', 'A14', NULL, NULL),
(1599101620559, 'VJ852', '2019-01-31', '202041100', 'Nguyễn Văn C', 'A89052', 'VN', 'C12', NULL, NULL),
(1599138217608, 'VJ822', '2019-01-02', '202041100', 'Nguyễn Văn C', 'A89052', 'VN', 'C12', NULL, NULL),
(1599138217719, 'VJ823', '2019-01-02', '202041101', 'Nguyễn Văn D', 'A89053', 'VN', 'B2', NULL, NULL),
(1599138217728, 'VJ840', '2019-01-02', '202041102', 'Nguyễn Văn E', 'A89054', 'VN', 'D6', NULL, NULL),
(1599138217735, 'VJ841', '2019-01-02', '202041103', 'Nguyễn Văn F', 'A89055', 'VN', 'E27', NULL, NULL),
(1599138217742, 'VJ850', '2019-01-02', '202041104', 'Nguyễn Văn G', 'A89056', 'VN', 'F18', NULL, NULL),
(1599138217748, 'VJ851', '2019-01-02', '202041105', 'Nguyễn Văn H', 'A89057', 'VN', 'A11', NULL, NULL),
(1599138217755, 'VJ855', '2019-01-02', '202041106', 'Nguyễn văn J', 'A89058', 'VN', 'B14', NULL, NULL),
(1599138217761, 'VJ886', '2019-01-02', '202041107', 'Nguyễn Văn K', 'A89059', 'VN', 'A15', NULL, NULL),
(1599138217768, 'VJ860', '2019-01-02', '202041108', 'Nguyễn Văn L', 'A89060', 'VN', 'E5', NULL, NULL),
(1599138217772, 'VJ860', '2019-01-02', '202041109', 'Nguyễn Văn M', 'A89061', 'VN', 'D7', NULL, NULL),
(1599138217775, 'VJ860', '2019-01-02', '202041110', 'Nguyễn Văn N', 'A89062', 'VN', 'F7', NULL, NULL),
(1599138217779, 'VJ860', '2019-01-02', '202041111', 'Nguyễn Văn Q', 'A89063', 'VN', 'C8', NULL, NULL),
(1599138217785, 'VJ861', '2019-01-02', '202041112', 'Nguyễn Văn P', 'A89064', 'VN', 'C29', NULL, NULL),
(1599138217788, 'VJ861', '2019-01-02', '202041113', 'Nguyễn Văn R', 'A89065', 'VN', 'C21', NULL, NULL),
(1599138217792, 'VJ861', '2019-01-02', '202041114', 'Nguyễn Văn S', 'A89066', 'VN', 'E15', NULL, NULL),
(1599496127976, 'VJ811, VJ812', '2020-09-07', '11111', 'My', '1111111111', 'Vietnam', 'A3', NULL, NULL),
(1599498838556, 'VJ811, VJ812', '2020-09-07', '11111', 'My', '1111111111', 'Vietnam', 'A1', NULL, NULL),
(1600831540842, 'VJ202', '2020-09-23', '202042020', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', 'VJ202', NULL),
(1600831695840, 'VJ202', '2020-09-23', '202042020', 'Nguyễn Văn B', 'A89051', 'VN', 'A13', 'VJ202', 0),
(1600919871191, 'VJ124', '2020-09-24', '202041098', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', NULL, NULL),
(1600919871206, 'VJ124', '2020-09-24', '202041099', 'Trần Thị B', 'A89051', 'VN', 'A13', NULL, NULL),
(1600920907760, 'VJ924', '2020-09-24', '202041999', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', NULL, NULL),
(1601000278983, 'VJ259', '2020-09-25', '202041098', 'Nguyễn Văn A', 'A89050', 'VN', 'A12', NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `wh_sell_detail`
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
-- Dumping data for table `wh_sell_detail`
--

INSERT INTO `wh_sell_detail` (`sdt_id`, `sl_id`, `product_code`, `sdt_sold_number`, `sdt_currency`, `sdt_price`, `flight_number`) VALUES
(1, 1599101620546, '0113800502', 1, 'USD', 33.6, 'VJ813'),
(2, 1599101620552, '0113800503', 1, 'USD', 30, 'VJ813'),
(3, 1599101620559, '0153600039', 1, 'USD', 49.2, 'VJ852'),
(4, 1599138217608, '0153600039', 1, 'USD', 49.2, NULL),
(5, 1599138217719, '0153600037', 1, 'USD', 36.6, NULL),
(6, 1599138217728, '0454800035', 1, 'USD', 33, NULL),
(7, 1599138217735, '0452100963', 1, 'USD', 51, NULL),
(8, 1599138217742, '0454800055', 1, 'USD', 19.2, NULL),
(9, 1599138217748, '0454800059', 1, 'USD', 24, NULL),
(10, 1599138217755, '04D8000001', 1, 'USD', 30, NULL),
(11, 1599138217761, '16F1100006', 1, 'USD', 9, NULL),
(12, 1599138217768, '15G6000002', 1, 'USD', 35.4, NULL),
(13, 1599138217772, '15G0300001', 1, 'USD', 33, NULL),
(14, 1599138217775, '15D4500002', 1, 'USD', 107.4, NULL),
(15, 1599138217779, '1596500141', 1, 'USD', 39, NULL),
(16, 1599138217785, '15G0400001', 1, 'USD', 33, NULL),
(17, 1599138217788, '16D2400017', 1, 'USD', 51, NULL),
(18, 1599138217792, '1655500083', 1, 'USD', 81, NULL),
(19, 1599496127976, '0454800059', 1, 'USD', 16, NULL),
(20, 1599498838556, '0113800503', 1, 'USD', 40, NULL),
(25, 1600831540842, '0113800502', 1, 'USD', 36.6, NULL),
(26, 1600831540842, '0113800503', 1, 'USD', 30, NULL),
(27, 1600831695840, '0153600037', 1, 'USD', 36.6, NULL),
(33, 1600919871191, '0186200001', 4, 'USD', 49.8, NULL),
(34, 1600919871191, '01D2200004', 2, 'USD', 6, NULL),
(35, 1600919871191, '0153600045', 4, 'USD', 55.2, NULL),
(36, 1600919871206, '0153600048', 4, 'USD', 66, NULL),
(37, 1600919871206, '0141500255', 9, 'USD', 31.2, NULL),
(38, 1600920907760, '0118100045', 5, 'USD', 36, NULL),
(39, 1601000278983, '0186200001', 1, 'USD', 30, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `wh_user`
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
-- Dumping data for table `wh_user`
--

INSERT INTO `wh_user` (`user_id`, `user_name`, `user_password`, `user_salt`, `user_full_name`, `user_enable`, `user_created_at`, `user_department_id`) VALUES
(1, 'backend', '1515E280EE877059667B63A766FA2B060EDA2A473D4518A11FAD69A32B1C33ECC27B0800A458ED733F4D65DCEEF83E936CE0EB5AFD7A517C4C27D96AE6C5C343', '45693129', 'Nguyen Backend', b'0', '2020-02-26 00:11:01', 1),
(2, 'frontend', '1BB211185D552870BF77F1C445ADCEC2731347967182D63EE4940E9C3623D6708AF1FF9B02EA4979F39BC099B817C48D2CE860FD4C8AAE02626F3C8D3611AC92', '56186786', 'Nguyen Frontend', b'0', '2020-02-26 00:10:58', 1),
(3, 'admin', '755067EDA539DDF4E48B5A8C979D9A3B21904963D52E5657B08470A6541F1D7C4C03B804EF90981CD44BCA66A88B8D9F6EA83251B0D433561610A56B4834DB94', '63550449', 'Administrator', b'1', '2020-02-18 09:53:52', 1),
(4, 'tuanngoanh@vietjetair.com', '7D1614E570238537C47CE8B32B12F4991BD2049D985ACEB6911D3CD9988DBCB201C6E225A7B4FA420B289A640F432FC521D4E0057699179BC51D5EE4D7BD2219', '77365398', 'Ngô Anh Tuấn', b'1', '2020-08-26 09:17:26', 2),
(5, 'huonghuynh@vietjetair.com', '43D8C6F45EB4BF5F6FF46C035AFC399A2333B530DECCC3EF12B8F5CE222F1FC8BD7408A4E17EB33FC93420B83066657E10BDC9ACD326E3926A31D817DF1551FA', '46898184', 'Huỳnh Thị Thu Hương', b'1', '2020-08-26 09:18:04', 2);

-- --------------------------------------------------------

--
-- Table structure for table `wh_user_permission`
--

CREATE TABLE `wh_user_permission` (
  `user_permission_id` int(11) NOT NULL,
  `user_permission_userid` int(11) DEFAULT NULL,
  `user_permission_permissionid` int(11) DEFAULT NULL,
  `user_permission_permissioncode` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `wh_user_permission`
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
(98, 5, 3, 'QLTK_TTKX');

-- --------------------------------------------------------

--
-- Table structure for table `wh_warehouse`
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
-- Indexes for dumped tables
--

--
-- Indexes for table `wh_airport`
--
ALTER TABLE `wh_airport`
  ADD PRIMARY KEY (`airport_code`);

--
-- Indexes for table `wh_cityPair`
--
ALTER TABLE `wh_cityPair`
  ADD PRIMARY KEY (`citypair_id`);

--
-- Indexes for table `wh_copy_seal`
--
ALTER TABLE `wh_copy_seal`
  ADD PRIMARY KEY (`citypair_id`);

--
-- Indexes for table `wh_declaration`
--
ALTER TABLE `wh_declaration`
  ADD PRIMARY KEY (`de_number`);

--
-- Indexes for table `wh_department`
--
ALTER TABLE `wh_department`
  ADD PRIMARY KEY (`department_id`);

--
-- Indexes for table `wh_department_permissions`
--
ALTER TABLE `wh_department_permissions`
  ADD PRIMARY KEY (`department_permission_id`),
  ADD KEY `fk_wh_department_permissions_wh_department` (`department_permission_departmentid`),
  ADD KEY `fk_wh_department_permissions_wh_permissions` (`department_permission_permissionid`);

--
-- Indexes for table `wh_destroy`
--
ALTER TABLE `wh_destroy`
  ADD PRIMARY KEY (`destroy_id`);

--
-- Indexes for table `wh_destroy_detail`
--
ALTER TABLE `wh_destroy_detail`
  ADD PRIMARY KEY (`destroy_detail_id`);

--
-- Indexes for table `wh_de_details`
--
ALTER TABLE `wh_de_details`
  ADD PRIMARY KEY (`dt_id`);

--
-- Indexes for table `wh_exchangerates`
--
ALTER TABLE `wh_exchangerates`
  ADD PRIMARY KEY (`exchangerate_id`);

--
-- Indexes for table `wh_file_upload`
--
ALTER TABLE `wh_file_upload`
  ADD PRIMARY KEY (`file_id`);

--
-- Indexes for table `wh_inventory`
--
ALTER TABLE `wh_inventory`
  ADD PRIMARY KEY (`in_id`);

--
-- Indexes for table `wh_menus`
--
ALTER TABLE `wh_menus`
  ADD PRIMARY KEY (`menu_id`);

--
-- Indexes for table `wh_menu_details`
--
ALTER TABLE `wh_menu_details`
  ADD PRIMARY KEY (`menu_detail_id`);

--
-- Indexes for table `wh_modules`
--
ALTER TABLE `wh_modules`
  ADD PRIMARY KEY (`module_id`);

--
-- Indexes for table `wh_permissions`
--
ALTER TABLE `wh_permissions`
  ADD PRIMARY KEY (`permission_id`),
  ADD KEY `fk_wh_permissions_wh_modules` (`permission_module_id`);

--
-- Indexes for table `wh_products`
--
ALTER TABLE `wh_products`
  ADD PRIMARY KEY (`product_code`);

--
-- Indexes for table `wh_seal`
--
ALTER TABLE `wh_seal`
  ADD PRIMARY KEY (`se_number`);

--
-- Indexes for table `wh_seal_detail`
--
ALTER TABLE `wh_seal_detail`
  ADD PRIMARY KEY (`sealdetail_id`);

--
-- Indexes for table `wh_seal_product`
--
ALTER TABLE `wh_seal_product`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `wh_sell`
--
ALTER TABLE `wh_sell`
  ADD PRIMARY KEY (`sl_id`);

--
-- Indexes for table `wh_sell_detail`
--
ALTER TABLE `wh_sell_detail`
  ADD PRIMARY KEY (`sdt_id`);

--
-- Indexes for table `wh_user`
--
ALTER TABLE `wh_user`
  ADD PRIMARY KEY (`user_id`);

--
-- Indexes for table `wh_user_permission`
--
ALTER TABLE `wh_user_permission`
  ADD PRIMARY KEY (`user_permission_id`),
  ADD KEY `fk_wh_user_permission_wh_user` (`user_permission_userid`),
  ADD KEY `fk_wh_user_permission_wh_permissions` (`user_permission_permissionid`);

--
-- Indexes for table `wh_warehouse`
--
ALTER TABLE `wh_warehouse`
  ADD PRIMARY KEY (`warehouse_id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `wh_cityPair`
--
ALTER TABLE `wh_cityPair`
  MODIFY `citypair_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT for table `wh_department`
--
ALTER TABLE `wh_department`
  MODIFY `department_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `wh_department_permissions`
--
ALTER TABLE `wh_department_permissions`
  MODIFY `department_permission_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `wh_destroy`
--
ALTER TABLE `wh_destroy`
  MODIFY `destroy_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `wh_destroy_detail`
--
ALTER TABLE `wh_destroy_detail`
  MODIFY `destroy_detail_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `wh_de_details`
--
ALTER TABLE `wh_de_details`
  MODIFY `dt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=66;

--
-- AUTO_INCREMENT for table `wh_file_upload`
--
ALTER TABLE `wh_file_upload`
  MODIFY `file_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=18;

--
-- AUTO_INCREMENT for table `wh_inventory`
--
ALTER TABLE `wh_inventory`
  MODIFY `in_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- AUTO_INCREMENT for table `wh_menus`
--
ALTER TABLE `wh_menus`
  MODIFY `menu_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `wh_menu_details`
--
ALTER TABLE `wh_menu_details`
  MODIFY `menu_detail_id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=74;

--
-- AUTO_INCREMENT for table `wh_modules`
--
ALTER TABLE `wh_modules`
  MODIFY `module_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `wh_permissions`
--
ALTER TABLE `wh_permissions`
  MODIFY `permission_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT for table `wh_seal_detail`
--
ALTER TABLE `wh_seal_detail`
  MODIFY `sealdetail_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=232;

--
-- AUTO_INCREMENT for table `wh_seal_product`
--
ALTER TABLE `wh_seal_product`
  MODIFY `id` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=232;

--
-- AUTO_INCREMENT for table `wh_sell_detail`
--
ALTER TABLE `wh_sell_detail`
  MODIFY `sdt_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=41;

--
-- AUTO_INCREMENT for table `wh_user`
--
ALTER TABLE `wh_user`
  MODIFY `user_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `wh_user_permission`
--
ALTER TABLE `wh_user_permission`
  MODIFY `user_permission_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=99;

--
-- AUTO_INCREMENT for table `wh_warehouse`
--
ALTER TABLE `wh_warehouse`
  MODIFY `warehouse_id` int(11) NOT NULL AUTO_INCREMENT;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
