   M S B A M L   `   `   `  ���� F  BMusicPlayer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:MusicPlayerMusicPlayer  I EMusicPlayerAPI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG4clr-namespace:MusicPlayerAPI;assembly=MusicPlayerAPIMusicPlayerAPI O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem     MusicPlayer.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     +localclr-namespace:MusicPlayer     OMusicPlayerAPI4clr-namespace:MusicPlayerAPI;assembly=MusicPlayerAPI     9System&clr-namespace:System;assembly=mscorlib     -     �Name$  
mainWindow��$
��1130��$	��700��$	��460��$	��860�� � Icon$ Images/icon.ico�� � TaskbarItemInfo *  $System.Windows.Shell.TaskbarItemInfo     ThumbButtonInfos *  $System.Windows.Shell.ThumbButtonInfo  -   taskbarPrevButton��   ImageSource$ Images\prev_taskbar.png��   Description$ 
Назад��  -   taskbarPlayPauseButton��$ Images\play_taskbar.png��$  Воспроизвести��  -   taskBarNextButton��$ Images\next_taskbar.png��$ Вперёд�� � 	Resources    	TextBrush  ElementsBaseBrush  ActiveTextBrush  ActiveTextMouseOverBrush  ActiveTextMousePressedBrush  SongCellBorderBrush $  SongCellBackgroundMouseOverBrush  SongCellForegroundBrush   SongCellTitleForegroundBrush #	 SongCellForegroundSelectedBrush !
 SearchInactiveForegroundBrush  SearchBorderBrush  MenuBackgroundBrush  MenuForegroundBrush   MenuForegroundMouseOverBrush " FavoritesButtonBackgroundBrush  FavoritesButtonBorderBrush  NavigItemForegroundBrush % !NavigItemForegroundMouseOverBrush " TabItemForegroundSelectedBrush  ScrollBarBackgroundBrush ! ScrollBarRepeatMouseOverBrush $  ScrollBarRepeatMousePressedBrush  ScrollBarRepeatPathBrush % !ScrollBarRepeatPathMouseOverBrush  ScrollBarThumbBrush   ScrollBarThumbMouseOverBrush  PlayerButtonMouseOverBrush  PlayerButtonMouseDownBrush  PlayerBackgroundBrush  PlayerSliderBackgroundBrush % !PlayerSliderBackgroundFilledBrush    PlayerSliderThumbStrokeBrush !! PlayerSongInfoForegroundBrush " ScrollBarLineButton �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� CornerRadius F�Name �� Path �� RelativeSource �� Triggers P� Property P� Value �� 
TargetName # ScrollBarPageButton $ ScrollBarThumb % VerticalScrollBar � RowDefinitions ]� DecreaseRepeatButton ]� Thumb ]� IncreaseRepeatButton �� Triggers & ComboBoxToggleButton � ColumnDefinitions ��Name ]� 
Conditions �� Property �� Value ' MenuBackground ( ComboBoxTextBox �� 	ClickMode d� 	IsChecked �� Mode  J� IsHitTestVisible! �� Text" ��Name# �� 
IsReadOnly$ �� 
IsReadOnly% �� IsDropDownOpen& � AllowsTransparency' J� SnapsToDevicePixels( � ActualWidth) �� MaxDropDownHeight* �� Padding+ K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation,   DirectionalNavigation- P� 
SourceName. �� 
SourceName/ ��Name ) favoritesButtonStyle0 �Name1 ��Name2 �� ScaleX3 �� ScaleY4 ]�Name5 � Cursor6 ��Name7 � ToolTip8 �� ElementName9 ��Name: �� Value; �� Binding<   TabNavigation= ��Name> K� ZIndex?   TabIndex * textButtonStyle@ ��Name + playerButtonStyle , contentButtonImageA �� StretchDirection - sliderButtonStyle . sliderThumbStyleB A�Name'  !System.Windows.VisualStateManagerC   VisualStateGroups%  System.Windows.VisualStateGroupD   States   System.Windows.VisualStateE   
StoryboardF �� 
TargetNameG �� TargetProperty	H W� ToI g� Duration / sliderStyleJ ]�NameK n�Name 0 contextMenuStyle 1 contextMenuItemStyleL ��NameM m� InputGestureText&   System.Windows.Controls.DataGrid*  $System.Windows.Controls.DataGridCellN �� Orientation��%�1  &	        &	      &	 
     &	      &	      &	      &	      &	 #     &	 (     &		 -     &	
 2     &	 7     &	 <     &	 A     &	 F     &	 K     &	 P     &	 U     &	 Z     &	 _     &	 d     &	 i     &	 n     &	 s     &	 x     &	 }     &	 �     &	 �     &	 �     &	 �     &	 �     &	 �     &	  �     &	! �     &	" �     &	# �    &	$ V    &	% �    7 " 7 # 7 $ 7 # 7 " '�� i    7 % &	& 7    &	( �    '�� (	    7 ( '�� �    &	) �    '�� �    '�� `    '�� ^    &	* &    &	+ �    &	,      &	- 2     &	. �     &	/ C#    7 - 7 . 7 - &	0 k%    &	1 '    '  �(    �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� " ��.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  � |�	 .  ��  � ��	 �� "
 ��.R��� $��Border��$��0q�$ 2��
������ 
������ $��0q�._�F� $
 Path��$��Center=�$��Center=�#���  ���� $ Content� �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ Border��#	 �   � ���� $
 Path��#	 �   � ��P�  � �� . . ��� $ Border��#	 �   � ���� $
 Path��#	 �   � ���� " ��.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  � ��	 .  ��  � |�	 .  ��  � ��	 �� "
 ��.R��� 
������ �� " n�.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  � ��	 .  ��  � |�	 .  ��  � ��	 �� "
 n�.R��� $��Border��$ 2��#��z��#��z��$��0q� P�  �@J�IsMouseOver . . ��� $ Border��#	 �   � ��P�  �@J�IsMouseCaptureWithin . . ��� $ Border��#	 �   � ���� "
 ��.R��  �� $��14���� $��0.00001* ��� $��14��.+��� $��3��$ 2��#���  �� $��0��8��  $��14��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����.  �� 8�� $��ScrollBar.PageUpCommand�� n� 8�� $��0q�#���   �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��14��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 .  P�  �@��Orientation$ Vertical=�. ���  � ��$	 14����  � ��$
	 Auto����  � ��8	   �� "
 d�.R��  �� .+��� $ Border��$��2��
������ 
������ $��0q� ]�  ��  �@J�IsMouseOver . ��  �@d�	IsChecked$
 TrueY�./��� $ Border��#	 �   � ��]�  ��  �@J�IsMouseOver .  ��  �@d�	IsChecked$
 TrueY�./��� $ Border��#	 � '  � ��P�  �@d�	IsChecked$
 TrueY�. ��� $ Border�� � ��$	 1,1,1,0q��� $ Border��#	 �   � ��P�  � {� .  �� "
 ��.R��� $ PART_ContentHost��|�.  #��z���� " ��.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  � ��$
	 Auto=���  � ��$
	 Auto=���  � ��	 . ��  � ��$		 120����  � ��$	 20����  �@�Cursor$
	 Hand����  � ��	 �� "
 ��.R�� .+�d� $��ToggleButton��#��� & $��2��|�.  $ Press=� �� $ IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+�� $��ContentSite��  .  #��z! $��8,3,23,0q�$��Center=�$
��Left=�#���  �� $" PART_EditableTextBox��8��  $��30��$��Center=�$
��Left=�$��Center=�$��3,3,23,3q�|�. $y�Hidden=�#$ z# ��Z� � $��Popup��$��Bottom=�#��z% & . |�.  $��Slide=�.��� $��DropDown��' . #��z( $��0,0,0,4q�#��z) .+��� $ DropDownBorder��#���  $	* 0,4q�$��1,0,1,1q�#���  �� ' . .���� + . $, 	Contained=� P� $- ToggleButton�� �@J�IsMouseOver . . ��� $ ContentSite��#	 �   � ��]�  �� $. ToggleButton�� �@J�IsMouseOver .  �� $. ToggleButton�� �@d�	IsChecked$
 TrueY�./��� $ ContentSite��#	 �   � ��]�  �� $. ToggleButton�� �@J�IsMouseOver . �� $. ToggleButton�� �@d�	IsChecked$
 TrueY�./��� $ ContentSite��#	 �   � ��P�  �@��HasItems .  . ��� $ DropDownBorder�� � ��$	 95��P�  � {� .  P�  �@��
IsGrouping . . ���  � ��	 .  P� $- Popup�� �@�AllowsTransparency . . ��� $ DropDownBorder�� � ��$	 	0,-20,0,0q�P�  �@��
IsEditable . . ���  � ��	 .  �� $ PART_EditableTextBox�� � y�$	 Visible=��� $ ContentSite�� � y�$	 Hidden=� �� " ��.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  �@�Cursor$
	 Hand����  � ��	 �� "
 ��.R��� $ Border��$* 4q�$��0,0,0,0q�' . 
������ ._��� $/ contentPresenter��#��z��#���   P�  �@J�IsMouseOver . . ��� $ contentPresenter��#	 �   � ���� $ Border��#	 �   � ���� $ Border�� � ��
	 ���� P�  �@��
IsSelected . . ��� $ Border�� � y�$	 	Collapsed=��� " ��.���  � ��	 �� *Width+��  � ��	 �� *Height+��  �@�Cursor$
	 Hand����  � |�	 .  ��  � ��	 �� "
 ��.R�� $0 
buttonGrid��
������ .+��� $ buttonSurface��$ 4��$��1q�#���  
������ �� $1 buttonCaption��$��Center=�$��Center=�#��z�� P�  �@J�IsMouseOver . . ��� $ buttonSurface��#	 �   � ��P�  � �� . . ��� $ buttonCaption�� �@J�RenderTransformOrigin$	 0.5,0.5&��� $ buttonCaption�� � z�	 �� $	2 0.9Q�$	3 0.9Q��� " ��.���  �@�OverridesDefaultStyle	 .  ��  �@�Cursor	 �� *
CursorType+��  � ��	 �� "
 ��.R��� $ navigItemBorder��-   
������ #���  $��1q�$��2,0,2,4q�$ 4��._�]� $4 outer��$��6q���. 
������ 5 �� *
CursorType+.+��� $6 addDel��-   $��Right=�#��� ) 
������ $y�Hidden=�$W7 QДобавить в избранное / удалить из избранного��$
5 Hand������ $8 btnImg��$ Source.Height����� $8 btnImg��$ Source.Width�.���� $9 btnImg������ *AddRemoveFavoriteImageSource+]� $4 inner��$
��Left=���. 
������ $	��300������ *Height+5 �� *
CursosType+.+��� $
��Left=����� *ImageSource+���� *Height+�� $/ label��$��Center=�#���  $��10,0,0,0q����� *Name+���� *
FontSize+���� *Height+ �� $: False��; �� *CanBeFavorite+.a��� $ addDel�� � ��$	 0���� $ addDel�� � ��$	 0���� $
: True��; �� *CanBeFavorite+.a��� $ addDel��#	 � )  � ��P�  �@J�IsMouseOver . . ��� $ navigItemBorder��#	 �   � ���� $ label��#	 �   � ��P�  � {� .  �� " ��.���  �@�OverridesDefaultStyle	 . ��  �@J�SnapsToDevicePixels	 . ��  �@��TabStripPlacement$	 Bottom=���  � ��
	 ���� ��  � ��	 �� "
 ��.R�� $< Local=� �� $��* ��� $
��Auto �.+��� $= tabControlHeaderPanel��$��1��$> 1��$��0,0,0,0q�+ . $? 1��
������ �� $ tabControlBorder��$��0��$��0q�$ 0��$< Local=�$, 	Contained=�$? 2��
������ #���  ._��� $1 PART_SelectedContentHost��$��4q�$��SelectedContent���� " ��.���  � ��	 �� "
 ��.R�� $
0 Root��.+��� $ Border��$��0,0,0,0q�$��1,1,1,1q�$ 2��#���  
������ $
5 Hand��._��� $1 ContentSite��$��Center=�$��Center=�$��Header��$��	12,2,12,2q���.  P�  �@��
IsSelected .  . ��� #	 �   � ��P�  �@��
IsSelected . . ��� $ Border�� �@�Cursor$	 Arrow���� $ Border��#	 �   � ���� $ Border��#	 �   � ���� #	 �   � ���� " ��.���  � ��	 �� *Width+��  � ��	 �� *Height+��  � ��
	 ���� ��  �@�Cursor$
	 Hand����  � |�	 .  ��  � ��	 �� "
 ��.R�� $0 
buttonGrid��
������ .+��� $@ buttonSurface��$��0��
������ �� $/ buttonCaption��$��Center=�$��Center=�#��z��#���   P�  �@J�IsMouseOver . . ��� $ buttonCaption��#	 �   � ��P�  � �� . . ��� $ buttonCaption��#	 �   � ���� " ��.���  � ��$	 60����  � ��$
	 Auto����  �@�Cursor$
	 Hand����  � |�	 .  ��  � ��	 �� "
 ��.R�� $0 
buttonGrid��
������ .+��� $@ buttonSurface���� $1 buttonCaption��$��Center=�$��Center=�#��z�� P�  �@J�IsMouseOver . . ��� $ buttonSurface��#	 �   � ��P�  � �� . . ��� $ buttonSurface��#	 �   � ���� $ buttonCaption�� �@J�RenderTransformOrigin$	 0.5,0.5&��� $ buttonCaption�� � z�	 �� $	2 0.9Q�$	3 0.9Q��� $��Uniform=�$A DownOnly=��� " ��.���  �@J�SnapsToDevicePixels	 . ��  � ��	 .  ��  � |�	 .  ��  � ��	 �� "
 ��.R��� 
������ �� " n�.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  � ��	 �� "
 n�.R�A� $B ellipseThumb��#���  $��16��$��16��$��Center=�#���   $��2��C   CommonStates��.D   
Normal��.E �� $F ellipseThumb��.��W� $G Height�$H 16X�$I 	0:0:0:0.1D�W� $G Width�$H 16X�$I 	0:0:0:0.1D�  	MouseOver��.E �� $F ellipseThumb��.��W� $G Height�$H 18X�$I 	0:0:0:0.1D�W� $G Width�$H 18X�$I 	0:0:0:0.1D�  Pressed��.E �� $F ellipseThumb��.��W� $G Height�$H 18X�$I 	0:0:0:0.1D�W� $G Width�$H 18X�$I 	0:0:0:0.1D�  Disabled���� " ��.���  � |�	 .  ��  �@��IsMoveToPointEnabled	 . ��  �@�Cursor$
	 Hand����  �@��IsSelectionRangeEnabled	 . ��  �@��SelectionEnd	 �� *Value+ �� *Self+��  � ��	 �� "
 ��.R��  �� $
��Auto �#��z��.+��� $ TrackBackground��$��0q�$ 0��$��2��$��0��$��0q�#���  ��Z� ]� $J 
PART_Track��$��0�� �� 8��   n� $K thumb��8�� {�.   �� 8�� �� $@ PART_SelectionRange��$��0��$
��Left=�#���  ���� $8 TrackBackground��$ Height��� " ��.���  �@J�SnapsToDevicePixels	 . ��  �@�OverridesDefaultStyle	 . ��  �@�IsSharedSizeScope	 . ��  �@��HasDropShadow	 .  ��  � ��$	 14(���  � ��	 �� "
 ��.R��� $ contextMenuBorder��#���  $��1q�#���  $
��Left=�._��� + . $, Cycle=� P�  �@��HasDropShadow .  . ��� $ contextMenuBorder�� �@��Padding$	 0,4,0,4q��� " m�.���  �@�OverridesDefaultStyle	 . ��  �@�Cursor$
	 Hand����  � ��	 �� "
 m�.R��� $ contextMenuItemBorder��._�� 
������ .+��� $L contextMenuItemTextBox��#��zM $��6,3,20,3q�#���  $��Center=� P�  �@m�IsHighlighted . . ��� $ contextMenuItemTextBox��#	 �   � ���� $ contextMenuItemBorder��#	 �   � ���� $ contextMenuItemBorder�� � ��
	 ���� P�  � {� .  �� "  .���  � |�	 .  ��  �@ 
IsReadOnly	 . ��  � ��
	 ���� ��  � ��
	 ���� ��  � ��$	 0q���  �@ SelectionMode$	 Single=���  � ��	 .  �� # �@ VerticalScrollBarVisibility$	 Visible=��� % �@ HorizontalScrollBarVisibility$	 Disabled=���  �@ GridLinesVisibility$
	 None=���  �@ GridLinesVisibility$
	 None=���   �@ HorizontalGridLinesBrush	 Z� ��  �@ VerticalGridLinesBrush	 Z� ��  �@ HeadersVisibility$
	 None=���  �@ AutoGenerateColumns	 .  ��  �@ CanUserResizeColumns	 .  ��  �@ CanUserReorderColumns	 .  ��  �@ CanUserSortColumns	 .  ��  �@ 	RowHeight$	 70Q���  �@ CanUserResizeRows	 .  ��  �@ RowBackground
	 ���� ��   �@ AlternatingRowBackground
	 ���� ��  �@ 	CellStyle	 �� "  ���  � |�	 .  ��  � ��$	 	10,0,10,0q���  � ��$	 0q���  � ��
	 ���� ��  � ��#	 �  ��  �@�Cursor$
	 Hand����  � ��	 �� "
  .R��� $ Border��
������ $ 4��#���  $��0,0,0,1q�._�]� |�.  ��.  .+��� $L durTextBlock��$��Right=�$��Center=�$��40������ *
Duration+�� $
��Left=�$N Vertical=�$��Center=�$��10,0,0,0q�.+��� $L titleTextBlock��$��0,0,0,0q�$��16(�#���  ���� *Title+�� $L artistTextBlock��$��0,4,0,0q����� *Artist+ P�  �@ 
IsSelected . . ��� $ Border��#	 �   � ���� #	 � 	  � ���� $ titleTextBlock��#	 � 	  � ����  �@�Cursor$	 Arrow��]�  ��  �@J�IsMouseOver . ��  �@ 
IsSelected .  ./��� $ Border��#	 �   � ��.��]� -   $4 mainDockPanel����. .+��� -   $ playerBorder��$��Bottom=�$��74��._�]� -	   $4 playerDockPanel��#���  ��. $��0,0,0,0q�.+��� -
   O ��Name$O controlButtonsStackPanel��$
��Left=�$N 
Horizontal=�$��Center=�$��0,0,0,0q�.+��� -   $6 
prevButton��$
��Left=�#��[+ .���� $��Images\prev.png��$��Uniform=�$A DownOnly=��� -   $6 playPauseButton��$
��Left=�#��[+ .���� $��Images\play.png��$��Uniform=�$A DownOnly=��� -   $6 
nextButton��$
��Left=�#��[+ .���� $��Images\next.png��$��Uniform=�$A DownOnly=��� -   $6 
randButton��$
��Left=�#��[+ .���� $��Images\rand.png��$��Uniform=�$A DownOnly=��� -   $6 repeatButton��$
��Left=�#��[+ .���� $��Images\repeat.png��$��Uniform=�$A DownOnly=��� -   $6 currentListButton��$
��Left=�$E7 ?К текущему списку воспроизведения��#��[+ .���� $��Images\current_songs.png��$��Uniform=�$A DownOnly=��� -   $6 songMenuButton��$
��Left=�#��[+ $%7 Меню аудиозаписи��.���� $��Images\settings.png��$��Uniform=�$A DownOnly=��� $N Vertical=�$��10, 0, 10, 0q�.+��� -   $O infoStackPanel��$	��Top=�$N 
Horizontal=�$��32��$��0,6,0,0q�.+��� -   $/ artistLabel��$��0,4,0,5q�$�� ��$
��Left=�$	��Top=�$��16(�#��� ! �� -   $/ 
titleLabel��$��0,4,0,5q�$�� ��$
��Left=�$��10,0,0,0q�$	��Top=�$��16(�#��� ! ]� -   $4 controlsDockPanel��$��Bottom=���. .+��� -   P ��Name$P volumeSlider��$��Right=�$��Center=�$��0,0,0,0q�$��Center=�$��80��Q � Value$Q 1Q�#��[/ R � Maximum$R 1Q�S � LargeChange$	S 0.1Q�T �� IsMoveToPointEnabledT . |�.  �� -   $6 volumeButton��$��Right=�$��Center=�$��28��$��Center=�$��0,0,0,0q�|�.  ���� " ��.���  �@�Cursor$
	 Hand����  � ��	 �� "
 ��.R�� $0 volumeButtonGrid��
������ .+��� $@ volumeButtonSurface���� U ��Name$U volumeButtonCaption��$��Center=�|�.  $��Center=�#��z�� P�  � �� . . ��� $ volumeButtonCaption�� �@J�RenderTransformOrigin$	 0.5,0.5&��� $ volumeButtonCaption�� � z�	 �� $	2 0.9Q�$	3 0.9Q�.���� $��Images\volume_on.png��$��Uniform=�$A DownOnly=��� -   $/ maxTimelinePosLabel��$��Right=�$��00:00��$
��Left=�$��Center=�$��39��$��0,0,0,0q�#��� ! �� -   $/ 
slashLabel��$�� /��$��Right=�$
��Left=�$��Center=�$��19��$��0,0,0,0q�#��� ! �� -   $/ currTimelinePosLabel��$��Right=�$��00:00��$
��Left=�$��Center=�$��38��$��0,0,0,0q�#��� ! �� -   $P musicTimelineSlider��V � SmallChange$V 1Q�$��Center=�T . $��0,0,6,0q�|�.  #��[/ r� -   W r�Name$W mediaElement��$
��Left=�$��49��$��-54,500,0,-48q�$	��Top=�$��38��X r� UnloadedBehavior$X Manual=�Y r� Stretch$
Y Fill=�Z r� Volume$Z 1Q�[ r� LoadedBehavior$[ Manual=�]� -   $4 mainAreaDockPanel����.  .+��� -   $ navigBorder��$��0q�
������ ._�]� -   $4 navigDockPanel��$��0q�$	��360����. ���� *ActualHeight+$8 mainAreaDockPanel��$ OneWay=�.+�� -    \ �Name$\ navigProgressBar��$	��Top=�$��0q�$��2��#���  ��Z� ��Z� �� -!   $O modeStackPanel��$	��Top=�$��	18,0,10,0q�$N 
Horizontal=�$��36��$y�Hidden=�.+��� -"   $L modeTextBlock��#���   $��Режим: ��$��0,2,0,0q�$��Center=�$��13.333(��� -#   ] ��Name$] modeComboBox��
������ |�.  $��13.333(��� -$   $" addressTextBox��$	��Top=�$��
16,2,16,-1q�$��30��$��Bottom=�$��0q�$��0q�$ . #���   ^ �� 
CaretBrush#^ �   #���  _ J� 	AllowDrop_ .  $��13.333(���Z� �� -%   $O themeStackPanel��$��Bottom=�$��	18,4,10,5q�$N 
Horizontal=�$��36��.+��� -&   $L themeTextBlock��#���   $'��!Стиль оформления: ��$��0,2,0,0q�$��Center=�$��13.333(��� -'   $] themeComboBox��|�.  $��13.333(�` �� SelectedIndex$` 0��.@��� Светлый�� Темный�� -(   a ��Name$a navigTabControl��b �� TabStripPlacement$b Bottom=�$��	10,0,10,0q�$��1q�
������ |�.  .@��� $	��167��$��26��$��13.333(�$��8,0,0,0q�.���� -)   c ��Name$c navigListView��
������ $��Disabled=�$	��332����.  $
��Auto=�|�.  ��Z� �� $	��167��$��26��$��13.333(�.���� -*   $c favoritesListView��
������ $��Disabled=�$	��332��$
��Auto=�|�.  ��.  ��Z� �� -+   $ audioParamsBorder��$	��Top=�$��0q�
������ $��0q�._�]� -,   $4 audioParamsStackPanel��$��73��.+�]� $	��Top=���.  $��	10,0,10,0q�.+��� --   $L sortTextBlock��$
��Left=�$��Center=�$��Сортировка: ��$��0,3,0,0q�#���   $��13.333(��� -.   $] sortComboBox��$
��Left=�$��36��$	��169��$��0q�$��13.333(�
������ $��0,2,0,2q�|�.  .@��� $"По дате добавления�� По названию�� По исполнителю�� -/   $ searchBorder��$��Right=�$��30��._�]� -0   $4 searchStackPanel����. .+��� -1   $6 searchButton��$��Right=�$��27��$
��🔎��#���  $��13.333(�#��� * $7 
Поиск��|�.  $��0,1,1,1q����� *ActualHeight+$8 searchTextBox��$ OneWay=��� -2   $" searchTextBox��$��16(�$��Center=�$��0q�$	��300��
������ $47 .Введите поисковой запрос��$��Поиск...��
������ $
��Left=�#��� 
 #^ �  ���� " ��.���  � ��$	 1q���  � ��#	 �   P�  �@J�IsMouseOver . . ���  � ��#	 �  �� ; �� $8 searchTextBox��$ 	IsFocused�: True.a���  � ��#	 �  ]� $��Bottom=���.  $��	10,0,10,0q�.+��� -3   $/ numOfAudioTextBlock��$
��Left=�$��0q�$8��2Количество песен в списке: 0��$��13.333(�$
��Left=�$��Disabled=�$��0,3,0,0q�
������ $��0q�
������ #���   $��Center=��� -4   $6 myMusicButton��$��Right=�$��Моя музыка��$��0q�$��13.333(�
������ |�.  #��� * ��Z� � -5   $\ songsProgressBar��$	��Top=�$��0,0,0,0q�$��2��#���  ��Z� ��Z�   -6   d  Name$d playlist1DataGrid��$y�Visible=����� *ActualWidth+$8 audioParamsStackPanel��$ OneWay=�e   Columnse 4	  .System.Windows.Controls.DataGridTemplateColumn	  ,
  &System.Windows.Controls.DataGridColumnf 
  Width5  /System.Windows.Controls.DataGridLengthConverter$f *   -7   $d playlist2DataGrid��$y�	Collapsed=����� *ActualWidth+$8 audioParamsStackPanel��$ OneWay=�e 	  $f * 