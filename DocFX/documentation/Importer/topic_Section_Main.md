# Main Section

The root area has some general settings.  
![Root Section](../../images/img_Unity_Root.png)

### Main Grid Size
This number dictates what all of the instantiated tilesets/entities scales will change their scale to, in case several LDtk layer's GridSize's are different from one another.  
Set this as the base size to work from in regards to scale.

**Note:** Upon first time importing the LDtk project or when the importer is reset, this value will default to the `DefaultGridSize` defined in LDtk.

### Sprite Atlas
Create your own [**Sprite Atlas**](https://docs.unity3d.com/Manual/class-SpriteAtlas.html) and assign it if desired.  
All tileset art that is used in levels will be packed to the Sprite Atlas.

- This solves [**tilemap tearing**](../Topics/topic_TilemapTearing.md). 
- The sprite atlas is reserved for auto-generated sprites only; Any foreign sprites assigned to the atlas will be removed.
- Only the individual tiles that are used are packed, resulting in optimal atlas size.
- This field will only appear if any tilesets are defined in the LDtk project.

### Custom Level Prefab
Optional. This prefab is instantiated as the root GameObject for all levels in the build process.  
Whether the field is assigned or not, the instantiated GameObject will have a [**Fields**](../Topics/topic_Fields.md) component added for getting the level's fields.  
Use this prefab field as a primary means of executing custom events upon import with the [**import interfaces**](../Topics/topic_ImportEventInterfaces.md).

### De-parent In Runtime
If this is set on, then the project, levels, and entity-layer GameObjects will have components that act to de-parent all of their children on start.  
This results in increased runtime performance by minimizing the hierarchy depth.  
Keep this on if the exact level/layer hierarchy structure is not a concern in runtime.  
[**Article about this particular optimization**](https://blogs.unity3d.com/2017/06/29/best-practices-from-the-spotlight-team-optimizing-the-hierarchy/)

### Render IntGrid Values
When this is enabled, all [**IntGrid Tiles**](../Topics/topic_IntGridTile.md) will have their tile's sprite rendered.
This toggle will only appear if any IntGrid layers are defined.

### Use Composite Collider
Use this to add a CompositeCollider2D to all IntGrid tilemaps.   
This can help with slowdown due to very large levels.  




