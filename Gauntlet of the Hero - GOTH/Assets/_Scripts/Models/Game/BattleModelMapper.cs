using System;
using System.Collections.Generic;
using System.Linq;
using GauntletOfTheHero.Backend.Models;

namespace GauntletOfTheHero.Gameplay.Models
{
    public static class BattleModelMapper
    {
        public static HeroModel ToHeroModel(HeroDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new HeroModel
            {
                id = dto.id,
                name = dto.name,
                stats = new CharacterStatsModel
                {
                    maxHealth = dto.maxHealth,
                    currentHealth = dto.currentHealth,
                    attack = dto.attack,
                    defense = dto.defense,
                    magic = dto.magic
                },
                moves = dto.moves?.Select(ToMoveModel).ToList() ?? new List<MoveModel>()
            };
        }

        public static MonsterModel ToMonsterModel(MonsterDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new MonsterModel
            {
                id = dto.id,
                name = dto.name,
                difficultyRank = dto.difficultyRank,
                stats = new CharacterStatsModel
                {
                    maxHealth = dto.maxHealth,
                    currentHealth = dto.maxHealth,
                    attack = dto.attack,
                    defense = dto.defense,
                    magic = dto.magic
                }
            };
        }

        public static MoveModel ToMoveModel(MoveDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new MoveModel
            {
                id = dto.id,
                name = dto.name,
                moveType = dto.moveType,
                effectType = dto.effectType,
                power = dto.power,
                buffAttribute = dto.buffAttribute,
                buffAmount = dto.buffAmount,
                buffDuration = dto.buffDuration
            };
        }

        public static BattleSnapshotModel ToSnapshot(BattleSessionDto sessionDto)
        {
            if (sessionDto == null)
            {
                return null;
            }

            // Prefer battleState values for initial snapshot (overrides from run context)
            var battleState = sessionDto.battleState;

            var heroModel = new HeroModel
            {
                id = sessionDto.hero.id,
                name = sessionDto.hero.name,
                stats = new CharacterStatsModel
                {
                    maxHealth = battleState?.heroMaxHealth ?? sessionDto.hero.maxHealth,
                    currentHealth = battleState?.heroCurrentHealth ?? sessionDto.hero.currentHealth,
                    attack = battleState?.heroAttack ?? sessionDto.hero.attack,
                    defense = battleState?.heroDefense ?? sessionDto.hero.defense,
                    magic = battleState?.heroMagic ?? sessionDto.hero.magic
                },
                moves = sessionDto.hero.moves?.Select(ToMoveModel).ToList() ?? new List<MoveModel>()
            };

            var monsterModel = new MonsterModel
            {
                id = sessionDto.monster.id,
                name = sessionDto.monster.name,
                difficultyRank = sessionDto.monster.difficultyRank,
                stats = new CharacterStatsModel
                {
                    maxHealth = sessionDto.monster.maxHealth,
                    currentHealth = battleState?.monsterCurrentHealth ?? sessionDto.monster.maxHealth,
                    attack = battleState?.monsterAttack ?? sessionDto.monster.attack,
                    defense = battleState?.monsterDefense ?? sessionDto.monster.defense,
                    magic = battleState?.monsterMagic ?? sessionDto.monster.magic
                }
            };

            return new BattleSnapshotModel
            {
                sessionId = sessionDto.sessionId,
                hero = heroModel,
                monster = monsterModel,
                activeBuffs = sessionDto.battleState?.activeBuffs?.Select(ToBuffModel).ToList() ?? new List<ActiveBuffModel>(),
                logs = new List<string>(),
                heroMoveName = null,
                monsterMoveName = null,
                battleFinished = false,
                winner = "NONE"
            };
        }

        public static BattleSnapshotModel ToSnapshot(Guid sessionId, BattleSnapshotModel previousSnapshot, BattleTurnResultDto turnDto)
        {
            if (turnDto == null)
            {
                return previousSnapshot;
            }

            HeroModel hero = previousSnapshot?.hero;
            MonsterModel monster = previousSnapshot?.monster;

            if (hero?.stats != null)
            {
                hero.stats.currentHealth = turnDto.heroCurrentHealth;
                hero.stats.attack = turnDto.heroAttack;
                hero.stats.defense = turnDto.heroDefense;
                hero.stats.magic = turnDto.heroMagic;
            }

            if (monster?.stats != null)
            {
                monster.stats.currentHealth = turnDto.monsterCurrentHealth;
                monster.stats.attack = turnDto.monsterAttack;
                monster.stats.defense = turnDto.monsterDefense;
                monster.stats.magic = turnDto.monsterMagic;
            }

            return new BattleSnapshotModel
            {
                sessionId = sessionId,
                hero = hero,
                monster = monster,
                activeBuffs = turnDto.activeBuffs?.Select(ToBuffModel).ToList() ?? new List<ActiveBuffModel>(),
                logs = turnDto.logs ?? new List<string>(),
                heroMoveName = turnDto.heroMoveName,
                monsterMoveName = turnDto.monsterMoveName,
                battleFinished = turnDto.battleFinished,
                winner = turnDto.winner
            };
        }

        private static ActiveBuffModel ToBuffModel(ActiveBuffDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new ActiveBuffModel
            {
                targetSide = dto.targetSide,
                attribute = dto.attribute,
                amount = dto.amount,
                remainingTurns = dto.remainingTurns
            };
        }
    }
}