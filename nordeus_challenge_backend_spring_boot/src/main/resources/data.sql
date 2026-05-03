INSERT INTO moves (id, name, move_type, effect_type, power, buff_attribute, buff_target, buff_amount, buff_duration) VALUES
(1, 'Slash', 'PHYSICAL', 'DAMAGE', 16, NULL, NULL, 0, 0),
(2, 'Shield Up', 'PHYSICAL', 'BUFF', 0, 'DEFENSE', 'SELF', 5, 2),
(3, 'Battle Cry', 'PHYSICAL', 'BUFF', 0, 'ATTACK', 'SELF', 5, 2),
(4, 'Second Wind', 'MAGIC', 'HEAL', 14, NULL, NULL, 0, 0),
(5, 'Shadow Bolt', 'MAGIC', 'DAMAGE', 20, NULL, NULL, 0, 0),
(6, 'Drain Life', 'MAGIC', 'HEAL', 10, NULL, NULL, 0, 0),
(7, 'Curse', 'MAGIC', 'BUFF', 0, 'ATTACK', 'OPPONENT', -4, 2),
(8, 'Dark Pact', 'MAGIC', 'BUFF', 0, 'MAGIC', 'SELF', 6, 2),
(9, 'Bite', 'PHYSICAL', 'DAMAGE', 17, NULL, NULL, 0, 0),
(10, 'Web Throw', 'PHYSICAL', 'BUFF', 0, 'DEFENSE', 'OPPONENT', -4, 2),
(11, 'Pounce', 'PHYSICAL', 'DAMAGE', 24, NULL, NULL, 0, 0),
(12, 'Skitter', 'PHYSICAL', 'BUFF', 0, 'DEFENSE', 'SELF', 6, 2),
(13, 'Flame Breath', 'MAGIC', 'DAMAGE', 28, NULL, NULL, 0, 0),
(14, 'Claw Swipe', 'PHYSICAL', 'DAMAGE', 20, NULL, NULL, 0, 0),
(15, 'Intimidate', 'PHYSICAL', 'BUFF', 0, 'ATTACK', 'OPPONENT', -5, 2),
(16, 'Dragon Scales', 'PHYSICAL', 'BUFF', 0, 'DEFENSE', 'SELF', 7, 2),
(17, 'Rusty Blade', 'PHYSICAL', 'DAMAGE', 19, NULL, NULL, 0, 0),
(18, 'Dirty Kick', 'PHYSICAL', 'BUFF', 0, 'DEFENSE', 'OPPONENT', -5, 2),
(19, 'Frenzy', 'PHYSICAL', 'BUFF', 0, 'ATTACK', 'SELF', 6, 2),
(20, 'Headbutt', 'PHYSICAL', 'DAMAGE', 25, NULL, NULL, 0, 0),
(21, 'Firebolt', 'MAGIC', 'DAMAGE', 21, NULL, NULL, 0, 0),
(22, 'Arcane Surge', 'MAGIC', 'BUFF', 0, 'MAGIC', 'SELF', 6, 2),
(23, 'Mana Drain', 'MAGIC', 'BUFF', 0, 'MAGIC', 'OPPONENT', -4, 2),
(24, 'Hex Shield', 'MAGIC', 'BUFF', 0, 'DEFENSE', 'SELF', 6, 2);

INSERT INTO heroes (id, name, max_health, current_health, attack, defense, magic) VALUES
(1, 'Knight of Dawn', 120, 120, 18, 14, 10);

INSERT INTO hero_moves (hero_id, move_id) VALUES
(1, 1),
(1, 2),
(1, 3),
(1, 4);

INSERT INTO monsters (id, name, difficulty_rank, max_health, current_health, attack, defense, magic) VALUES
(1, 'Witch', 1, 80, 80, 10, 8, 18),
(2, 'Dragon', 2, 110, 110, 16, 13, 17),
(3, 'Giant Spider', 3, 130, 130, 20, 15, 14),
(4, 'Goblin Warrior', 4, 150, 150, 24, 18, 12),
(5, 'Goblin Mage', 5, 175, 175, 18, 20, 26);

INSERT INTO monster_moves (monster_id, move_id) VALUES
(1, 5),
(1, 6),
(1, 7),
(1, 8),
(2, 9),
(2, 10),
(2, 11),
(2, 12),
(3, 13),
(3, 14),
(3, 15),
(3, 16),
(4, 17),
(4, 18),
(4, 19),
(4, 20),
(5, 21),
(5, 22),
(5, 23),
(5, 24);
