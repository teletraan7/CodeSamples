// Copyright @2021 Jonathon Neal
/*
JON'S NOTES: Okay so here is something to remember. If i want to access the servers version of a specific player controller I need to start the process in the PCon.
Remember, you tried starting in the player character and learned that it would always then access the servers PCon 0. I believe it's because since you have already entered the server before calling for the PCon
You are dgetting the PCon for the servers proxy for your character. Which would be the servers fist PCon.
It might also be that an issue with the current way I'm getting the player controller
*/

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "PlayerCharacter.h"
#include "Net/UnrealNetwork.h"
#include "MainPlayerController.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FPlayerHackingStart);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FPlayerHackingEnd);

class AItemDropOffPoint;

UCLASS()
class DOUBLECROSS_API AMainPlayerController : public APlayerController
{
	GENERATED_BODY()
public:
	AMainPlayerController();
	virtual void SetupInputComponent() override;
	void SetPlayerEnabledState(bool SetPlayerEnabled);
	UFUNCTION(Server, WithValidation, Reliable)
	void ServerItemPickup(AInteractableActorBase* ItemBeingPickUp);
	bool ServerItemPickup_Validate(AInteractableActorBase* ItemBeingPickUp);
	void ServerItemPickup_Implementation(AInteractableActorBase* ItemBeingPickUp);
	UFUNCTION(Server, Reliable)
	void ServerItemDropOff(AItemDropOffPoint* DropPoint);
	void ServerItemDropOff_Implementation(AItemDropOffPoint* DropPoint);
	FPlayerHackingStart HackPressed;
	FPlayerHackingEnd HackReleased;
	UItemHolderComponent* GetItemHolder() const
	{
		return ItemHolderComp;
	}
private:
	int PlayerTeam{0};
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, meta=( AllowPrivateAccess = true))
	APlayerCharacter* PCharacter;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UItemHolderComponent* ItemHolderComp;
	void HandleAttackInput();
	void HandleInteractInput();
	void HandleHackInputPress();
	void HandleHackInputRelease();
};