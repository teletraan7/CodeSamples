// Copyright @2020 Jonathon Neal


#include "MainPlayerController.h"
#include "Pawns/PawnPlayer.h"

#pragma region INPUT METHODS
AMainPlayerController::AMainPlayerController()
{
	ItemHolderComp = CreateDefaultSubobject<UItemHolderComponent>(TEXT("Item Holder"));	
}

void AMainPlayerController::SetupInputComponent()
{
    Super::SetupInputComponent();
    //Action Inputs
    InputComponent->BindAction("Attack", IE_Pressed , this, &AMainPlayerController::HandleAttackInput);
    //InputComponent->BindAction("Interact", IE_Pressed , this, &AMainPlayerController::HandleInteractInput);
    InputComponent->BindAction("Hack", IE_Pressed , this, &AMainPlayerController::HandleHackInputPress);
    InputComponent->BindAction("Hack", IE_Released , this, &AMainPlayerController::HandleHackInputRelease);
}

void AMainPlayerController::SetPlayerEnabledState(bool SetPlayerEnabled)
{
    if (SetPlayerEnabled)
    {
        GetPawn()->EnableInput(this);
    }
    else
    {
        GetPawn()->DisableInput(this);
    }

    bShowMouseCursor = !SetPlayerEnabled;
    SetShowMouseCursor(bShowMouseCursor);
}

void AMainPlayerController::HandleAttackInput()
{
    if (PCharacter) PCharacter->Attack();
}

void AMainPlayerController::HandleInteractInput()
{
    if (PCharacter) PCharacter->Interact();
}

void AMainPlayerController::HandleHackInputPress()
{
    HackPressed.Broadcast();
}

void AMainPlayerController::HandleHackInputRelease()
{
    HackReleased.Broadcast();
}
#pragma endregion 