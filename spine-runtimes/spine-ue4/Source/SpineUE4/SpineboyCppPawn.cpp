// Fill out your copyright notice in the Description page of Project Settings.

#include "SpineUE4.h"
#include "SpineSkeletonAnimationComponent.h"
#include "SpineboyCppPawn.h"


// Sets default values
ASpineboyCppPawn::ASpineboyCppPawn()
{
 	// Set this pawn to call Tick() every frame. You can turn this off to improve performance if you don't need it.
	PrimaryActorTick.bCanEverTick = true;

}

// Called when the game starts or when spawned
void ASpineboyCppPawn::BeginPlay()
{
	Super::BeginPlay();
	USpineSkeletonAnimationComponent* animation = FindComponentByClass<USpineSkeletonAnimationComponent>();	
	animation->SetAnimation(0, FString("walk"), true);
}

// Called every frame
void ASpineboyCppPawn::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	USpineSkeletonAnimationComponent* animation = FindComponentByClass<USpineSkeletonAnimationComponent>();
	spine::AnimationState *state = animation->GetAnimationState();
	spine::TrackEntry *entry = state->getCurrent(0);
	if (entry) {
		GEngine->AddOnScreenDebugMessage(-1, 0.5f, FColor::Yellow, FString(entry->getAnimation()->getName().buffer()));
	}
}

// Called to bind functionality to input
void ASpineboyCppPawn::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

}

