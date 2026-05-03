package com.mmileticc.nordeuschallenge;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication
@EnableScheduling
public class NordeuschallengeApplication {

	public static void main(String[] args) {
		SpringApplication.run(NordeuschallengeApplication.class, args);
	}

}
