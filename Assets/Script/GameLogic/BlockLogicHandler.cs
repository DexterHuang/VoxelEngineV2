using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BlockLogicHandler {
    public static void onTick() {
        foreach (Chunk chunk in Chunk.displayedChunks) {
            for (int i = 0; i < chunk.damagedBlocksIndexs.Count; i++) {
                Block block = chunk.getBlock(chunk.damagedBlocksIndexs[i]);
                block.damageBlock(-10);
            }
        }
    }

}
