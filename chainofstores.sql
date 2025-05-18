-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1
-- Время создания: Май 18 2025 г., 19:32
-- Версия сервера: 10.4.32-MariaDB
-- Версия PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `chainofstores`
--

DELIMITER $$
--
-- Процедуры
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `calculate_statistics` ()   BEGIN
    DECLARE v_average_sale DECIMAL(10, 2);
    DECLARE v_sum_sale DECIMAL(10, 2);
    DECLARE v_max_sale DECIMAL(10, 2);
    DECLARE v_min_sale DECIMAL(10, 2);
    
    -- Рассчитываем описательные статистики
    SELECT AVG(sale_price), SUM(sale_price), MAX(sale_price), MIN(sale_price)
    INTO v_average_sale, v_sum_sale, v_max_sale, v_min_sale
    FROM sales;
    
    -- Вставляем рассчитанные значения в таблицу statistic_results
    INSERT INTO statistic_results (date_of_calc, average_sale, sum_sale, max_sale, min_sale)
    VALUES (CURDATE(), v_average_sale, v_sum_sale, v_max_sale, v_min_sale);
END$$

--
-- Функции
--
CREATE DEFINER=`root`@`localhost` FUNCTION `count_positions_by_condition` (`condition_value` DECIMAL(10,2)) RETURNS INT(11) DETERMINISTIC BEGIN
    DECLARE result_count INT;
    
    -- Считаем количество позиций, удовлетворяющих условию
    SELECT COUNT(*)
    INTO result_count
    FROM sales
    WHERE sale_price > condition_value;
    
    RETURN result_count;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `categories`
--

CREATE TABLE `categories` (
  `id_category` int(15) NOT NULL,
  `category_name` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `categories`
--

INSERT INTO `categories` (`id_category`, `category_name`) VALUES
(1, 'Парфюмерия'),
(2, 'Уход за лицом1'),
(3, 'Уход за телом'),
(4, 'Уход за волосами'),
(5, 'Декоративная косметика');

-- --------------------------------------------------------

--
-- Структура таблицы `leftovers_in_storages`
--

CREATE TABLE `leftovers_in_storages` (
  `id_leftover` int(15) NOT NULL,
  `id_product` int(15) NOT NULL,
  `id_storage` int(15) NOT NULL,
  `update_data` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `leftovers_in_storages`
--

INSERT INTO `leftovers_in_storages` (`id_leftover`, `id_product`, `id_storage`, `update_data`) VALUES
(2, 2, 2, '2024-04-19'),
(3, 3, 3, '2024-12-23'),
(4, 4, 4, '2024-09-14'),
(5, 5, 5, '2023-05-11'),
(10, 2, 2, '2024-11-10'),
(11, 2, 2, '2024-11-23'),
(12, 2, 2, '2024-11-10'),
(13, 2, 2, '2024-11-23'),
(14, 2, 2, '2024-11-16'),
(15, 2, 2, '2024-11-24'),
(16, 2, 2, '2024-11-16'),
(17, 2, 2, '2024-11-24'),
(18, 3, 2, '2024-11-12'),
(19, 3, 2, '2024-11-05'),
(20, 3, 2, '2024-11-12'),
(21, 3, 2, '2024-11-05'),
(22, 4, 2, '2024-11-05'),
(23, 5, 2, '2024-11-16'),
(24, 4, 2, '2024-11-05'),
(25, 5, 2, '2024-11-16'),
(26, 3, 2, '2024-11-03'),
(27, 4, 2, '2024-11-11'),
(28, 2, 2, '2024-11-24'),
(29, 4, 2, '2024-11-05'),
(30, 5, 2, '2024-11-12'),
(31, 4, 2, '2024-11-05'),
(32, 5, 2, '2024-11-12'),
(36, 2, 2, '2025-05-06'),
(37, 9, 3, '2025-05-13'),
(38, 2, 3, '2025-05-13'),
(39, 3, 3, '2025-05-13'),
(40, 4, 5, '2025-05-13'),
(41, 5, 5, '2025-05-13'),
(42, 6, 2, '2025-05-13'),
(43, 6, 4, '2025-05-13'),
(44, 7, 1, '2025-05-13'),
(45, 8, 3, '2025-05-13'),
(46, 9, 3, '2025-05-13'),
(47, 10, 1, '2025-05-13'),
(48, 10, 4, '2025-05-13'),
(49, 16, 3, '2025-05-13'),
(50, 12, 1, '2025-05-13'),
(51, 10, 5, '2025-05-13'),
(52, 8, 4, '2025-05-13'),
(53, 29, 3, '2025-05-13'),
(54, 29, 3, '2025-05-13'),
(55, 28, 2, '2025-05-13'),
(56, 27, 4, '2025-05-13'),
(57, 26, 3, '2025-05-13'),
(58, 25, 4, '2025-05-13'),
(59, 24, 5, '2025-05-13'),
(60, 23, 4, '2025-05-13'),
(61, 20, 5, '2025-05-13'),
(62, 22, 1, '2025-05-13'),
(63, 18, 1, '2025-05-13'),
(64, 16, 4, '2025-05-13'),
(65, 5, 5, '2025-05-13'),
(66, 18, 4, '2025-05-13'),
(67, 27, 3, '2025-05-13'),
(68, 12, 4, '2025-05-13'),
(69, 23, 4, '2025-05-13'),
(70, 21, 3, '2025-05-13'),
(71, 23, 2, '2025-05-13'),
(72, 9, 3, '2025-05-13'),
(73, 4, 3, '2025-05-13'),
(74, 28, 3, '2025-05-13'),
(75, 22, 4, '2025-05-13'),
(76, 19, 2, '2025-05-13'),
(77, 16, 4, '2025-05-13'),
(78, 9, 5, '2025-05-13'),
(79, 16, 3, '2025-05-13'),
(80, 27, 3, '2025-05-13'),
(81, 18, 3, '2025-05-13');

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `popular_products_price_changes`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `popular_products_price_changes` (
`товары у которых поменялась цена` varchar(20)
,`barcode` varchar(20)
,`old_price` int(15)
,`new_price` int(15)
,`date_of_change` date
);

-- --------------------------------------------------------

--
-- Структура таблицы `positions`
--

CREATE TABLE `positions` (
  `id_position` int(15) NOT NULL,
  `position_name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `positions`
--

INSERT INTO `positions` (`id_position`, `position_name`) VALUES
(1, 'Менеджер'),
(2, 'Директор'),
(3, 'Кассир'),
(4, 'Сотрудник склада'),
(5, 'Аналитик');

-- --------------------------------------------------------

--
-- Структура таблицы `price_history`
--

CREATE TABLE `price_history` (
  `id_price_history` int(15) NOT NULL,
  `date_of_change` date NOT NULL,
  `old_price` int(15) NOT NULL,
  `new_price` int(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_general_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `products`
--

CREATE TABLE `products` (
  `id_product` int(11) NOT NULL,
  `barcode` varchar(20) NOT NULL,
  `product_name` varchar(20) NOT NULL,
  `id_supplier` int(11) NOT NULL,
  `id_subcategory` int(11) NOT NULL,
  `price` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `products`
--

INSERT INTO `products` (`id_product`, `barcode`, `product_name`, `id_supplier`, `id_subcategory`, `price`) VALUES
(2, '374637', 'MISSHA perfect cover', 3, 1, 542),
(3, '573363', 'DEAR DAHLIA paradise', 4, 2, 657),
(4, '263523', 'LIKATO PROFESSIONAL ', 1, 2, 780),
(5, '253422', 'AURA JEWELRY element', 2, 3, 1550),
(6, '254325', 'RABANNE fame', 5, 3, 990),
(7, '234321', 'CASA LEGGERA dolce a', 2, 2, 330),
(8, '538744', '3INA the color lip o', 3, 4, 882),
(9, '473355', 'BELIF moisturizing b', 5, 5, 657),
(10, '968644', 'BESO BEACH de sal', 3, 6, 544),
(12, '456453', 'ERBORIAN cc eye', 1, 6, 544),
(13, '353233', 'EAU THERMALE AVENE e', 3, 7, 668),
(15, '234432', 'NAJ OLEARI colour fa', 4, 8, 650),
(16, '223222', 'D\'ALBA extra intensi', 2, 8, 1800),
(17, '876544', 'VERSACE pour homme ', 1, 9, 761),
(18, '233456', 'GUERLAIN terracotta ', 5, 9, 890),
(19, '574734', 'MANLY PRO ga103', 4, 10, 557),
(20, '242324', 'MOIRA diamond daze l', 1, 10, 467),
(21, '122323', 'CATRICE sun lover gl', 2, 11, 1200),
(22, '152425', 'DIOR rouge dior', 3, 11, 690),
(23, '857473', 'GUERLAIN kisskiss be', 4, 12, 1450),
(24, '623543', 'CLARINS lip comfort ', 5, 12, 1320),
(25, '123322', 'EVELINE botanic expe', 3, 13, 550),
(26, '283762', 'MAC macximal matte l', 3, 13, 887),
(27, '283746', 'YADAH cactus', 2, 14, 443),
(28, '254333', 'ESSENCE fix & last 1', 3, 15, 1100),
(29, '965847', 'WELEDA birch body sc', 5, 16, 456);

-- --------------------------------------------------------

--
-- Дублирующая структура для представления `products_price_changes`
-- (См. Ниже фактическое представление)
--
CREATE TABLE `products_price_changes` (
`товары у которых поменялась цена` varchar(20)
,`barcode` varchar(20)
,`old_price` int(15)
,`new_price` int(15)
,`date_of_change` date
);

-- --------------------------------------------------------

--
-- Структура таблицы `sales`
--

CREATE TABLE `sales` (
  `id_sale` int(15) NOT NULL,
  `id_product` int(15) NOT NULL,
  `sale_date` date DEFAULT NULL,
  `sale_price` int(11) DEFAULT NULL,
  `id_store` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `sales`
--

INSERT INTO `sales` (`id_sale`, `id_product`, `sale_date`, `sale_price`, `id_store`) VALUES
(2, 2, '2024-04-19', 800, 2),
(5, 5, '2023-05-11', 560, 5),
(6, 4, '2024-11-28', 301, 2),
(7, 3, '2024-11-28', 300, 2),
(8, 6, '2024-11-27', 321, 3),
(9, 2, '2024-12-11', 1201, 5),
(13, 4, '2024-12-05', 12222, 2),
(15, 4, '2024-12-12', 1231, 4),
(16, 4, '2024-12-05', 1234, 2),
(18, 23, '2024-06-17', 4532, 3),
(19, 20, '2024-08-12', 7643, 1),
(20, 21, '2024-10-30', 8742, 4),
(21, 27, '2024-12-25', 875, 5),
(22, 17, '2025-02-12', 7463, 1),
(23, 21, '2025-04-17', 234, 4),
(24, 2, '2025-02-20', 765, 3),
(25, 6, '2025-02-18', 654, 5),
(26, 29, '2024-12-17', 450, 3),
(27, 13, '2025-04-19', 123, 3),
(28, 20, '2025-05-12', 654, 1),
(29, 7, '2024-11-18', 345, 2),
(30, 4, '2025-04-14', 908, 5),
(31, 21, '2025-03-20', 234, 4),
(32, 10, '2025-01-20', 120, 5),
(33, 27, '2025-01-14', 340, 3),
(34, 29, '2025-03-22', 459, 1),
(35, 4, '2024-10-17', 786, 2),
(36, 19, '2024-09-20', 120, 4),
(37, 4, '2025-05-11', 654, 4),
(38, 12, '2024-08-13', 200, 4),
(39, 6, '2024-07-04', 321, 4),
(40, 10, '2024-05-10', 123, 2),
(41, 22, '2024-11-13', 2345, 1),
(42, 10, '2025-01-09', 102, 3),
(43, 10, '2024-07-15', 122, 4),
(44, 4, '2025-04-24', 345, 4),
(45, 5, '2025-02-19', 543, 2),
(46, 4, '2025-04-17', 432, 4),
(47, 7, '2025-03-13', 231, 5);

--
-- Триггеры `sales`
--
DELIMITER $$
CREATE TRIGGER `decrement_sales_counter` AFTER DELETE ON `sales` FOR EACH ROW BEGIN
    UPDATE sales_summary SET total_sales = total_sales - 1;
END
$$
DELIMITER ;
DELIMITER $$
CREATE TRIGGER `increment_sales_counter` AFTER INSERT ON `sales` FOR EACH ROW BEGIN
    UPDATE sales_summary SET total_sales = total_sales + 1;
END
$$
DELIMITER ;

-- --------------------------------------------------------

--
-- Структура таблицы `sales_summary`
--

CREATE TABLE `sales_summary` (
  `summary_id` int(11) NOT NULL,
  `total_sales` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `sales_summary`
--

INSERT INTO `sales_summary` (`summary_id`, `total_sales`) VALUES
(1, 32);

-- --------------------------------------------------------

--
-- Структура таблицы `statistic_results`
--

CREATE TABLE `statistic_results` (
  `id` int(11) NOT NULL,
  `date_of_calc` date NOT NULL,
  `average_sale` decimal(10,2) DEFAULT NULL,
  `sum_sale` decimal(10,2) DEFAULT NULL,
  `max_sale` decimal(10,2) DEFAULT NULL,
  `min_sale` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `statistic_results`
--

INSERT INTO `statistic_results` (`id`, `date_of_calc`, `average_sale`, `sum_sale`, `max_sale`, `min_sale`) VALUES
(1, '2024-11-28', 890.00, 4450.00, 2300.00, 340.00),
(2, '2024-11-28', 890.00, 4450.00, 2300.00, 340.00);

-- --------------------------------------------------------

--
-- Структура таблицы `storages`
--

CREATE TABLE `storages` (
  `id_storage` int(15) NOT NULL,
  `storage_name` varchar(30) NOT NULL,
  `storage_address` varchar(30) NOT NULL,
  `storage_phone_number` varchar(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `storages`
--

INSERT INTO `storages` (`id_storage`, `storage_name`, `storage_address`, `storage_phone_number`) VALUES
(1, 'Склад в Ломоносовском районе', 'Попова, 45', '89960849678'),
(2, 'Склад -Ленина', 'Ленина, 66', '89548732738'),
(3, 'Склад Малый', 'Индустриальная,78', '89327482273'),
(4, 'Склад Центральный', 'Грачева, 13', '89362748472'),
(5, 'Склад №1', 'Чанъаньцзе, 89', '89574636262');

-- --------------------------------------------------------

--
-- Структура таблицы `stores`
--

CREATE TABLE `stores` (
  `id_store` int(15) NOT NULL,
  `store_name` varchar(30) DEFAULT NULL,
  `store_address` varchar(30) NOT NULL,
  `id_storage` int(15) NOT NULL,
  `store_phone_number` varchar(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `stores`
--

INSERT INTO `stores` (`id_store`, `store_name`, `store_address`, `id_storage`, `store_phone_number`) VALUES
(1, 'Магазин в ТЦ Океан', 'Попова, 45', 1, '89960849678'),
(2, 'Магазин -Ленина', 'Ленина, 66', 2, '89548732738'),
(3, 'Магазин в ТЦ Алмаз', 'Индустриальная,77', 3, '89327482273'),
(4, 'Магазин Центральный', 'Грачева, 13', 4, '89362748472'),
(5, 'Магазин Универсам №2', 'Чанъаньцзе, 89', 5, '89574636262');

-- --------------------------------------------------------

--
-- Структура таблицы `subcategories`
--

CREATE TABLE `subcategories` (
  `id_subcategory` int(11) NOT NULL,
  `subcategory_name` varchar(30) NOT NULL,
  `id_category` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `subcategories`
--

INSERT INTO `subcategories` (`id_subcategory`, `subcategory_name`, `id_category`) VALUES
(1, 'женские ароматы', 1),
(2, 'мужские ароматы', 1),
(3, 'ароматы для дома', 1),
(4, 'помада', 5),
(5, 'тушь', 5),
(6, 'тени для век', 5),
(7, 'пудра для лица', 5),
(8, 'шампунь для волос', 4),
(9, 'спрей', 4),
(10, 'бальзам', 4),
(11, 'сыворотка для лица', 1),
(12, 'крем для лица', 2),
(13, 'очищающая пенка', 2),
(14, 'скраб для тела', 3),
(15, 'лосьон для тела', 3),
(16, 'пилинг для тела', 3),
(23, 'гель', 2),
(24, 'скраб для ног', 3);

-- --------------------------------------------------------

--
-- Структура таблицы `suppliers`
--

CREATE TABLE `suppliers` (
  `id_supplier` int(15) NOT NULL,
  `supplier_name` varchar(20) NOT NULL,
  `email` varchar(20) NOT NULL,
  `phone_number` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `suppliers`
--

INSERT INTO `suppliers` (`id_supplier`, `supplier_name`, `email`, `phone_number`) VALUES
(1, 'ИП Смирнов', 'smirnov@gmail.com', '89960849678'),
(2, 'ПромЛогистика', 'logpro@mail.ru', '89548732738'),
(3, 'ИндустрияПлюс', 'indplus@yandex.ru', '89327482273'),
(4, 'ТехноПартнер', 'technopart@gmail.com', '89362748472'),
(5, 'ИП Кузнецов', 'kuznetcov@mail.ru', '89574636262');

-- --------------------------------------------------------

--
-- Структура таблицы `system_users`
--

CREATE TABLE `system_users` (
  `id_user` int(11) NOT NULL,
  `id_position` int(11) NOT NULL,
  `user_name` varchar(50) NOT NULL,
  `user_surname` varchar(50) NOT NULL,
  `user_email` varchar(50) NOT NULL,
  `user_phone` varchar(50) NOT NULL,
  `login` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `system_users`
--

INSERT INTO `system_users` (`id_user`, `id_position`, `user_name`, `user_surname`, `user_email`, `user_phone`, `login`, `password`) VALUES
(1, 2, 'Дмитрий', 'Максимов', 'maximov@gmail.com', '+79985673478', 'dmitrmax5852', '74D?m63'),
(2, 1, 'Анна', 'Андреева', 'andreeva@yandex.ru', '+79864536267', 'annandre4567', '45An!909'),
(3, 4, 'Илья', 'Сидоров', 'sidorov@gmail.com', '+79948363625', 'sidoril9437', '35Si_73'),
(5, 3, 'Лилия', 'Боталова', 'botalova@yandex.ru', '+79957634598', 'botal7865', '56BLil7'),
(6, 5, 'Алексей', 'Романов', 'romanov@mail.ru', '+79763456765', 'roman7865', '12Ralek_'),
(7, 1, 'Дарья', 'Титова', 'titova@gmail.com', '+79964562398', 'titov3498', '54Tit?5');

-- --------------------------------------------------------

--
-- Структура таблицы `users`
--

CREATE TABLE `users` (
  `Id` int(30) NOT NULL,
  `Name` varchar(50) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `IsAdmin` varchar(10) NOT NULL,
  `IsActive` varchar(10) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп данных таблицы `users`
--

INSERT INTO `users` (`Id`, `Name`, `Password`, `IsAdmin`, `IsActive`) VALUES
(1, 'Вася', 'D033E22AE348AEB5660FC2140AEC35850C4DA997', 'True', 'True');

-- --------------------------------------------------------

--
-- Структура для представления `popular_products_price_changes`
--
DROP TABLE IF EXISTS `popular_products_price_changes`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `popular_products_price_changes`  AS SELECT `products`.`product_name` AS `товары у которых поменялась цена`, `products`.`barcode` AS `barcode`, `price_history`.`old_price` AS `old_price`, `price_history`.`new_price` AS `new_price`, `price_history`.`date_of_change` AS `date_of_change` FROM (`products` join `price_history` on(`products`.`id_product` = `price_history`.`id_price_history`)) WHERE `price_history`.`date_of_change` between '2023-01-01' and '2024-12-31' ORDER BY `products`.`product_name` ASC ;

-- --------------------------------------------------------

--
-- Структура для представления `products_price_changes`
--
DROP TABLE IF EXISTS `products_price_changes`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `products_price_changes`  AS SELECT `products`.`product_name` AS `товары у которых поменялась цена`, `products`.`barcode` AS `barcode`, `price_history`.`old_price` AS `old_price`, `price_history`.`new_price` AS `new_price`, `price_history`.`date_of_change` AS `date_of_change` FROM (`products` join `price_history` on(`products`.`id_product` = `price_history`.`id_price_history`)) WHERE `price_history`.`date_of_change` between '2023-01-01' and '2024-12-31' ORDER BY `products`.`product_name` ASC ;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `categories`
--
ALTER TABLE `categories`
  ADD PRIMARY KEY (`id_category`);

--
-- Индексы таблицы `leftovers_in_storages`
--
ALTER TABLE `leftovers_in_storages`
  ADD PRIMARY KEY (`id_leftover`),
  ADD KEY `id_product` (`id_product`),
  ADD KEY `id_storage` (`id_storage`);

--
-- Индексы таблицы `positions`
--
ALTER TABLE `positions`
  ADD PRIMARY KEY (`id_position`);

--
-- Индексы таблицы `price_history`
--
ALTER TABLE `price_history`
  ADD PRIMARY KEY (`id_price_history`);

--
-- Индексы таблицы `products`
--
ALTER TABLE `products`
  ADD PRIMARY KEY (`id_product`),
  ADD KEY `id_subcategory` (`id_subcategory`),
  ADD KEY `id_supplier` (`id_supplier`);

--
-- Индексы таблицы `sales`
--
ALTER TABLE `sales`
  ADD PRIMARY KEY (`id_sale`),
  ADD KEY `id_product` (`id_product`),
  ADD KEY `id_store` (`id_store`);

--
-- Индексы таблицы `sales_summary`
--
ALTER TABLE `sales_summary`
  ADD PRIMARY KEY (`summary_id`);

--
-- Индексы таблицы `statistic_results`
--
ALTER TABLE `statistic_results`
  ADD PRIMARY KEY (`id`);

--
-- Индексы таблицы `storages`
--
ALTER TABLE `storages`
  ADD PRIMARY KEY (`id_storage`);

--
-- Индексы таблицы `stores`
--
ALTER TABLE `stores`
  ADD PRIMARY KEY (`id_store`),
  ADD KEY `id_storage` (`id_storage`);

--
-- Индексы таблицы `subcategories`
--
ALTER TABLE `subcategories`
  ADD PRIMARY KEY (`id_subcategory`),
  ADD KEY `id_category` (`id_category`);

--
-- Индексы таблицы `suppliers`
--
ALTER TABLE `suppliers`
  ADD PRIMARY KEY (`id_supplier`);

--
-- Индексы таблицы `system_users`
--
ALTER TABLE `system_users`
  ADD PRIMARY KEY (`id_user`),
  ADD KEY `id_position` (`id_position`);

--
-- Индексы таблицы `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `categories`
--
ALTER TABLE `categories`
  MODIFY `id_category` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT для таблицы `leftovers_in_storages`
--
ALTER TABLE `leftovers_in_storages`
  MODIFY `id_leftover` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=82;

--
-- AUTO_INCREMENT для таблицы `positions`
--
ALTER TABLE `positions`
  MODIFY `id_position` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `price_history`
--
ALTER TABLE `price_history`
  MODIFY `id_price_history` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `products`
--
ALTER TABLE `products`
  MODIFY `id_product` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT для таблицы `sales`
--
ALTER TABLE `sales`
  MODIFY `id_sale` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=48;

--
-- AUTO_INCREMENT для таблицы `sales_summary`
--
ALTER TABLE `sales_summary`
  MODIFY `summary_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT для таблицы `statistic_results`
--
ALTER TABLE `statistic_results`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT для таблицы `storages`
--
ALTER TABLE `storages`
  MODIFY `id_storage` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT для таблицы `stores`
--
ALTER TABLE `stores`
  MODIFY `id_store` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT для таблицы `subcategories`
--
ALTER TABLE `subcategories`
  MODIFY `id_subcategory` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT для таблицы `suppliers`
--
ALTER TABLE `suppliers`
  MODIFY `id_supplier` int(15) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT для таблицы `system_users`
--
ALTER TABLE `system_users`
  MODIFY `id_user` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT для таблицы `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(30) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `leftovers_in_storages`
--
ALTER TABLE `leftovers_in_storages`
  ADD CONSTRAINT `leftovers_in_storages_ibfk_1` FOREIGN KEY (`id_product`) REFERENCES `products` (`id_product`),
  ADD CONSTRAINT `leftovers_in_storages_ibfk_2` FOREIGN KEY (`id_storage`) REFERENCES `storages` (`id_storage`);

--
-- Ограничения внешнего ключа таблицы `products`
--
ALTER TABLE `products`
  ADD CONSTRAINT `products_ibfk_2` FOREIGN KEY (`id_subcategory`) REFERENCES `subcategories` (`id_subcategory`),
  ADD CONSTRAINT `products_ibfk_3` FOREIGN KEY (`id_supplier`) REFERENCES `suppliers` (`id_supplier`);

--
-- Ограничения внешнего ключа таблицы `sales`
--
ALTER TABLE `sales`
  ADD CONSTRAINT `sales_ibfk_1` FOREIGN KEY (`id_product`) REFERENCES `products` (`id_product`),
  ADD CONSTRAINT `sales_ibfk_2` FOREIGN KEY (`id_store`) REFERENCES `stores` (`id_store`);

--
-- Ограничения внешнего ключа таблицы `stores`
--
ALTER TABLE `stores`
  ADD CONSTRAINT `stores_ibfk_1` FOREIGN KEY (`id_storage`) REFERENCES `storages` (`id_storage`);

--
-- Ограничения внешнего ключа таблицы `subcategories`
--
ALTER TABLE `subcategories`
  ADD CONSTRAINT `subcategories_ibfk_1` FOREIGN KEY (`id_category`) REFERENCES `categories` (`id_category`);

--
-- Ограничения внешнего ключа таблицы `system_users`
--
ALTER TABLE `system_users`
  ADD CONSTRAINT `system_users_ibfk_1` FOREIGN KEY (`id_position`) REFERENCES `positions` (`id_position`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
