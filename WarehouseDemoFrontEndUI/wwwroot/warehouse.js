window.drawWarehouse = (canvas, warehouse) => {
    if (!warehouse) return;
    const context = canvas.getContext("2d");
    context.clearRect(0, 0, canvas.width, canvas.height);

    // Draw grids
    warehouse.gridLocations.forEach(grid => {
        ctx.strokeStyle = "#ccc";
        ctx.strokeRect(grid.topLeft.x, grid.topLeft.y,
            grid.bottomRight.x - grid.topLeft.x,
            grid.bottomRight.y - grid.topLeft.y);
    });

    // Draw bots
    warehouse.activeBots.forEach(bot => {
        ctx.fillStyle = bot.color;
        ctx.fillRect(bot.position.x, bot.position.y, bot.size.x, bot.size.y);
    });

    // Draw warehouse border
    ctx.strokeStyle = "#000";
    ctx.strokeRect(warehouse.border.topLeft.x, warehouse.border.topLeft.y,
        warehouse.border.bottomRight.x - warehouse.border.topLeft.x,
        warehouse.border.bottomRight.y - warehouse.border.topLeft.y);
};