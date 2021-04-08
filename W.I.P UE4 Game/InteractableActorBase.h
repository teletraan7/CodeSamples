// Copyright @2021 Jonathon Neal

#pragma once

#include "CoreMinimal.h"
#include "GameplayTagAssetInterface.h"
#include "GameFramework/Actor.h"
#include "Components/BoxComponent.h"
#include "InteractableActorBase.generated.h"

UENUM(BlueprintType)
/// <summary>
/// Enums representing the types of interactable actors
/// </summary>
enum class EInteractionType : uint8 
{
	Security = 0 UMETA(DisplayName = "Security"),
	Stealable = 1 UMETA(DisplayName = "Stealable")
};

UCLASS()
/// <summary>
/// The base class for all actors players can interact with. Contains shared components, methods, and variables
/// </summary>
class DOUBLECROSS_API AInteractableActorBase : public AActor,public IGameplayTagAssetInterface
{
	GENERATED_BODY()
	
	public:	
	AInteractableActorBase();
	virtual void BeginPlay() override;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Interaction Type")
	EInteractionType InteractionType;
	
	protected:
	UFUNCTION()
	virtual void GetOwnedGameplayTags(FGameplayTagContainer& TagContainer) const override;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UStaticMeshComponent* ActorMesh;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UBoxComponent* ActorTriggerBox;
};