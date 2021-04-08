// Copyright @2020 Jonathon Neal


#include "InteractableActorBase.h"
#include "DoubleCross/PlayerCharacter.h"

AInteractableActorBase::AInteractableActorBase()
{
	PrimaryActorTick.bCanEverTick = false;
	ActorMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Actor Mesh"));
	RootComponent = ActorMesh;
	ActorTriggerBox = CreateDefaultSubobject<UBoxComponent>(TEXT("Actor Trigger Box"));
	ActorTriggerBox->SetupAttachment(RootComponent);
}

void AInteractableActorBase::BeginPlay()
{
	Super::BeginPlay();
}

void AInteractableActorBase::GetOwnedGameplayTags(FGameplayTagContainer& TagContainer) const
{
	UE_LOG(LogTemp, Error, TEXT("TEST"));
}