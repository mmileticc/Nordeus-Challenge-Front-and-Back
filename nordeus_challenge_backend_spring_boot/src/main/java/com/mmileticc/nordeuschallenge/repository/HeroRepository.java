package com.mmileticc.nordeuschallenge.repository;

import com.mmileticc.nordeuschallenge.model.entity.Hero;
import org.springframework.data.jpa.repository.JpaRepository;

public interface HeroRepository extends JpaRepository<Hero, Long> {
}
