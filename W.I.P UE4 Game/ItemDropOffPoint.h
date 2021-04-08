// Copyright @2021 Jonathon Neal

#pragma once

#include "CoreMinimal.h"
#include "DoubleCross/PlayerCharacter.h"
#include "DoubleCross/StealableActorBase.h"
#include "DoubleCross/ItemStruct.h"
#include "Net/UnrealNetwork.h"
#include "ItemDropOffPoint.generated.h"

UCLASS()
class DOUBLECROSS_API AItemDropOffPoint : public AActor
{
	GENERATED_BODY()

	public:
	AItemDropOffPoint();
	/// <summary>
	/// COLLECTING THE PLAYERS ITEMS ON THE SERVER
	/// </summary>
	/// <param name="PlayerItemHolder">The item holder for the player</param>
	void CollectItems(UItemHolderComponent* PlayerItemHolder);
	private:
	virtual void BeginPlay() override;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UStaticMeshComponent* ActorMesh;
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	UBoxComponent* ActorTriggerBox;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category="Components", meta = (AllowPrivateAccess = true))
	AActor* PlayerClass;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category="Items", meta = (AllowPrivateAccess = true), Replicated)
	TArray<FItemStruct> ItemsCollected;
	
	UFUNCTION()
	void OnOverlapBegin(UPrimitiveComponent* OverlappedComp, AActor* OtherActor, UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	/// <summary>
	/// ALERT THE PLAYER CONTROLLER THAT IT NEEDS TO CALL THE POINTS COLLECTION RPC. ONLY A CLIENT OWNED ACTOR CAN CALL A SERVER RPC. THIS IS NOT CLIENT OWNED
	/// </summary>
	/// <param name="Player">A reference to the player in the point</param>
	void AlertPlayerController(APlayerCharacter* Player);

	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override; 
};