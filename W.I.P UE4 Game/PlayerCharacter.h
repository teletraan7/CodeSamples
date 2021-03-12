// Copyright @2020 Jonathon Neal

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "GameplayTagAssetInterface.h"
#include "PaperFlipbookComponent.h"
#include "Camera/CameraComponent.h"
#include "DoubleCross/ItemHolderComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "PlayerCharacter.generated.h"

class ASecurityBarrierBase;
class AMainPlayerController;

UCLASS()
class DOUBLECROSS_API APlayerCharacter : public ACharacter, public IGameplayTagAssetInterface
{
	GENERATED_BODY()

public:
	APlayerCharacter();
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;
	bool bIgnoreMoveInput;
	void MoveVertical(float value);
	void MoveHorizontal(float value);
	void Interact();
	void Attack();
	void PickupItem(AInteractableActorBase* ItemBeingPickUp);
	UItemHolderComponent* GetItemHolder() const
	{
		return ItemBag;
	}

protected:
	virtual void BeginPlay() override;
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;
	
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Comoponents", meta = (AllowPrivateAccess = true))
	UCapsuleComponent* CapsuleComp;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Comoponents", meta = (AllowPrivateAccess = true))
	UPaperFlipbookComponent* SpriteComp;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Gameplay Tags")
	FGameplayTagContainer GameplayTags;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Gameplay Tag")
	FGameplayTag StealableTag;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Gameplay Tag")
	FGameplayTag SecurityTag;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UStaticMeshComponent* BagMesh;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UCameraComponent* CameraComp;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	USpringArmComponent* SpringComp;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UItemHolderComponent* ItemBag;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Animations", meta = (AllowPrivateAccess = true))
	UPaperFlipbook* IdleAnimation;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Animations", meta = (AllowPrivateAccess = true))
	UPaperFlipbook* WalkAnimation;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category="Values", meta = (AllowPrivateAccess = true));
	float MovementSpeed{0.0f};
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Objects Nearby", meta = (AllowPrivateAccess = true))
	TArray<AInteractableActorBase*> StolenItemsNearPawn;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Objects Nearby", meta = (AllowPrivateAccess = true))
	ASecurityBarrierBase* NearbySecurityObject{nullptr};

	FVector MovementDirection{0,0,0};
	FVector2D MovementValues{0,0};
	
	UFUNCTION()
    virtual void GetOwnedGameplayTags(FGameplayTagContainer& TagContainer) const override;

	//Animation State bool
	UFUNCTION(Server, Unreliable)
	void FlipSprite(float InputValue);
	void FlipSprite_Implementation(float InputValue);
	UPROPERTY(ReplicatedUsing=OnRep_IsFacingRight);
	bool IsFacingRight{false};
	UFUNCTION()
	void OnRep_IsFacingRight();
	UPROPERTY(ReplicatedUsing=OnRep_IsPlayerIdle);
	bool IsPlayerIdle{true};
	UFUNCTION()
	void OnRep_IsPlayerIdle();
	UFUNCTION(Server, Unreliable)
	void UpdateAnimation();
	void UpdateAnimation_Implementation();
	
	UFUNCTION()
    void OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
	UFUNCTION()
    void OnOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);
	static bool bIsActorInteractable(AActor* ActorInQuestion);
	void StoreSecurityRef(AInteractableActorBase* SecurityObj);
	void StoreNearbyObject(AInteractableActorBase* NearbyObject, bool bAddObj);
};