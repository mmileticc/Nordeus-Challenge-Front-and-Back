package com.mmileticc.nordeuschallenge.repository;

import com.mmileticc.nordeuschallenge.model.entity.Monster;
import org.springframework.data.jpa.repository.JpaRepository;

public interface MonsterRepository extends JpaRepository<Monster, Long> {
}
