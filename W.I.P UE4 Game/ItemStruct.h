// Copyright @2021 Jonathon Neal

#pragma once
#include "CoreMinimal.h"
#include "Engine/DataTable.h"
#include "ItemStruct.generated.h"

USTRUCT(BlueprintType)
/// <summary>
/// Struct to contain Item data from the item data table
/// </summary>
struct FItemStruct : public FTableRowBase
{
	GENERATED_USTRUCT_BODY()

	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = "Item")
	FString Name{""};
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = "Item")
	int Points{0};
};