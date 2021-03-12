// Copyright @2020 Jonathon Neal

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "PlayerCharacter.h"
#include "MainPlayerController.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FPlayerHackingStart);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FPlayerHackingEnd);

/*
*This player controller will handle player:
*	Team (Which team they are on, and how those interactions are handled)
*	Chat (Are they close enough for proximity chat, or using the radio chat)
*	States (Restrained, in a menu, visible)
*	Menu navigation
*/
UCLASS()
class DOUBLECROSS_API AMainPlayerController : public APlayerController
{
	GENERATED_BODY()
public:
	AMainPlayerController();
	virtual void SetupInputComponent() override;
	void SetPlayerEnabledState(bool SetPlayerEnabled);
	FPlayerHackingStart HackPressed;
	FPlayerHackingEnd HackReleased;
private:
	int PlayerTeam{0};
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, meta=( AllowPrivateAccess = true))
	APlayerCharacter* PCharacter;
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UItemHolderComponent* ItemHolderComp;
	void HandleAttackInput();
	void HandleInteractInput();
	void HandleHackInputPress();
	void HandleHackInputRelease();
};