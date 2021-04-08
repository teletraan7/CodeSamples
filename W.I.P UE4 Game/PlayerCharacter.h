// Copyright @2021 Jonathon Neal

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
	UFUNCTION(BlueprintCallable)
	/// <summary>
	/// CALLED FROM BLUEPRINTS TO MAKE AN EASY REFERENCE TO THIS CHARACTERS CONTROLLER. STILL NEEEDS TO BE FULL IMPLAMENTED
	/// </summary>
	/// <param name="PCon">THE PLAYER CONTROLLER FOR THIS CHARACTER</param>
	void SetPlayerCon(AMainPlayerController* PCon) 
	{
		PlayerCon = PCon;
	}
	AMainPlayerController* GetPlayerCon() const 
	{
		return PlayerCon;
	}

protected:
	virtual void BeginPlay() override;
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;
	

	//THESE CAN BE DELETED SINCE TAGS ARE NO LONGER BEING USED TO IDENTIFY INTERACTABLE ITEMS. DOUBLE CHECK FIRST
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Gameplay Tags")
	FGameplayTagContainer GameplayTags;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Gameplay Tag")
	FGameplayTag StealableTag;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Gameplay Tag")
	FGameplayTag SecurityTag;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Comoponents", meta = (AllowPrivateAccess = true))
	UCapsuleComponent* CapsuleComp;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Comoponents", meta = (AllowPrivateAccess = true))
	UPaperFlipbookComponent* SpriteComp;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UStaticMeshComponent* BagMesh;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UCameraComponent* CameraComp;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	USpringArmComponent* SpringComp;

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

	UPROPERTY(VisibleAnywhere, Category="Controller", meta = (AllowPrivateAccess = true))
	AMainPlayerController* PlayerCon;
	
	UFUNCTION()
    virtual void GetOwnedGameplayTags(FGameplayTagContainer& TagContainer) const override;

		
	UFUNCTION()
    void OnOverlapBegin(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
	UFUNCTION()
    void OnOverlapEnd(class UPrimitiveComponent* OverlappedComp, class AActor* OtherActor, class UPrimitiveComponent* OtherComp, int32 OtherBodyIndex);

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

	void StoreSecurityRef(AInteractableActorBase* SecurityObj);
	void StoreNearbyObject(AInteractableActorBase* NearbyObject, bool bAddObj);
};