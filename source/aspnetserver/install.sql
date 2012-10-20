DROP TABLE IF EXISTS `collection`;
CREATE TABLE `collection` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) NOT NULL,
  `public` tinyint(1) NOT NULL,
  `path` varchar(330) NOT NULL,
  `autoadd` tinyint(1) NOT NULL,
  `cachestatus` tinyint(4) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `public` (`public`),
  KEY `autoadd` (`autoadd`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `foldercache`;
CREATE TABLE `foldercache` (
  `id` int(11) NOT NULL,
  `content` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `collectionuser`;
CREATE TABLE `collectionuser` (
  `cid` int(11) NOT NULL,
  `uid` int(11) NOT NULL,
  `access` tinyint(1) NOT NULL,
  KEY `cid` (`cid`),
  KEY `uid` (`uid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `manga`;
CREATE TABLE `manga` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cid` int(11) NOT NULL,
  `title` varchar(100) NOT NULL,
  `path` varchar(330) NOT NULL,
  `type` tinyint(4) NOT NULL,
  `time` int(11) NOT NULL,
  `size` bigint(20) NOT NULL,
  `numpages` int(11) NOT NULL,
  `view` int(11) NOT NULL,
  `status` int(11) NOT NULL,
  `ltr` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `cid` (`cid`),
  KEY `path` (`path`),
  KEY `type` (`type`),
  KEY `status` (`status`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `mangacontent`;
CREATE TABLE `mangacontent` (
  `id` int(11) NOT NULL,
  `content` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = MYISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `mangadimensions`;
CREATE TABLE `mangadimensions` (
  `id` int(11) NOT NULL,
  `dimensions` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE = MYISAM DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `meta`;
CREATE TABLE `meta` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `mid` int(11) NOT NULL,
  `author` varchar(100) NOT NULL,
  `volume` int(11) NOT NULL,
  `series` varchar(100) NOT NULL,
  `year` int(11) NOT NULL,
  `publisher` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mid` (`mid`),
  KEY `series` (`series`),
  KEY `year` (`year`),
  KEY `publisher` (`publisher`),
  KEY `author` (`author`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `mangatag`;
CREATE TABLE `mangatag` (
  `mid` int(11) NOT NULL,
  `tid` int(11) NOT NULL,
  KEY `mid` (`mid`),
  KEY `tid` (`tid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `setting`;
CREATE TABLE `setting` (
  `name` varchar(30) NOT NULL,
  `value` text NOT NULL,
  PRIMARY KEY (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `tag`;
CREATE TABLE `tag` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(30) NOT NULL,
  `password` varchar(32) NOT NULL,
  `admin` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `username` (`username`),
  KEY `admin` (`admin`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `errorlog`;
CREATE TABLE `errorlog` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `time` int(11) NOT NULL,
  `type` varchar(50) NOT NULL,
  `source` varchar(100) NOT NULL,
  `message` varchar(200) NOT NULL,
  `stacktrace` text NOT NULL,
  PRIMARY KEY (`id`),
  KEY `time` (`time`),
  KEY `type` (`type`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8;
