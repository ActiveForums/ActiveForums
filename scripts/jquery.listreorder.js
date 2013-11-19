(function($){

$.fn.ListReorder = function (options) {
	
	$.fn.ListReorder.defaults = {
		itemHoverClass : 'itemHover',
		dragTargetClass : 'dragTarget',
		dropTargetClass : 'dropTarget',
		dragHandleClass : 'dragHandle',
		useDefaultDragHandle : true
	};

	var opts = $.extend({}, $.fn.ListReorder.defaults, options);
	
	return this.each(function() {
		var theList = $(this),   // The list (<ul>|<ol>)
			theItems = $('li.candrag', theList), // All <li> elements in the list
			dragActive = false,          // Are we currently dragging an item?
			dropTarget = null,           // The list placeholder
			dragTarget = null,           // The element currently being dragged
			dropIndex = -1,           	 // The list index of the dropTarget
			offset = {},				 // Positions the mouse in the dragTarget
			listOrder = [],				 // Keeps track of order relative to original order
			ref = this;
	
		theList.mouseout(ul_mouseout);
		
		// Create the drag target
		dragTarget = $('<div>&nbsp;</div>');
		dragTarget.insertAfter(theList);
		dragTarget.hide();
		dragTarget.css('position', 'absolute');
		dragTarget.addClass(opts.dragTargetClass);
		
		for (var i = 0; i < theItems.length; i++)
			listOrder.push(i);
		
		resetList();
	
		function resetList() {	
			theItems = $('li.candrag', theList),
			
			// For each <li> in the list
			theItems.each(function() {
				var li = $(this);
				
				var dragHandle = $('<span></span>');
				dragHandle.addClass(opts.dragHandleClass)
					.mouseover(li_mouseover)
					.mousedown(dragHandle_mousedown);
				
				if (opts.useDefaultDragHandle)
					dragHandle.css({
						'display' : 'block',
						'float' :  'left',
						'width' :  '16px',
						'cursor' : 'move'
					});
					
				$('.' + opts.dragHandleClass, li).remove();
				li.prepend(dragHandle);
			});
			
			clearListItemStyles();
		}
		
		// Return all list items to their default state
		function clearListItemStyles() {
			theItems.each(function() {
				var li = $(this);
				li.removeClass(opts.itemHoverClass);
				li.removeClass(opts.dropTargetClass);
			});
		}
		
		// Handle any cleanup when the mouse leaves the list
		function ul_mouseout() {
			if (!dragActive)
				clearListItemStyles();
		}
		
		// Add a hover class to a list item on mouseover
		function li_mouseover() {
			if (!dragActive) {
				clearListItemStyles();
				$(this).parent().addClass(opts.itemHoverClass);
			}
		}
		
		// Prepare the list for dragging an item
		function dragHandle_mousedown(e) {
			var li = $(this).parent();
			
			dragActive = true;
			dropIndex = theItems.index(li);
			
			// Show the drag target
			dragTarget.html(li.html());
			dragTarget.css('display', 'block');
			offset.top = e.pageY - li.offset().top;
			offset.left = e.pageX - li.offset().left;
			updateDragTargetPos(e);
			
			// Insert the placeholder
			dropTarget = li;
			dropTarget.html('');
			dropTarget.css('height', dragTarget.css('height'));
			dragTarget.css('width', dropTarget.width() + 'px');
			dropTarget.addClass(opts.dropTargetClass);
			
			// Disable Text and DOM selection
			//$(document).disableTextSelect();
			
			$(document).mouseup(dragHandle_mouseup);
			$(document).mousemove(document_mousemove);	
		}
		
		// If this were on the element, we could lose the drag on the element 
		// if we move the mouse too fast
		function document_mousemove(e) {
			if (dragActive) {
				// drag target follows mouse cursor
				updateDragTargetPos(e);
				
				// Don't do mess with drop index if we are above or below the list
				if (y_mid(dragTarget) > y_bot(theList) 
					|| y_mid(dragTarget) < y_top(theList)) {
					return;
				}
				
				// detect position of drag target relative to list items
				// and swap drop target and neighboring item if necessary
				if (y_mid(dragTarget) + 5 < y_top(dropTarget)) {
					swapListItems(dropIndex, --dropIndex);
				} else if (y_mid(dragTarget) - 5 > y_bot(dropTarget)) {
					swapListItems(dropIndex, ++dropIndex);
				}
			}
		}
		
		function dragHandle_mouseup() {
			// Restore the drop target
			dropTarget.html(dragTarget.html());
			dropTarget.removeClass(opts.dragTargetClass);
			dropTarget = null;
			
			// Hide the drag target
			dragTarget.css('display', 'none');
			
			dragActive = false;
			dragTarget.unbind('mouseup', dragHandle_mouseup);
			$(document).unbind('mousemove', document_mousemove);
			resetList();
			
			theList.trigger('listorderchanged', [theList, listOrder]);
			
			// Re-enable text selection
			//$(document).enableTextSelect();
			$(document).unbind('mouseup', dragHandle_mouseup);
		}
		
		function updateDragTargetPos(e) {
			dragTarget.css({ 
				'top' : e.pageY - offset.top + 'px',
				'left' : e.pageX - offset.left + 'px'
			});
		}
		
		// Change the order of two list items
		function swapListItems(oldDropIndex, newDropIndex) {
			// keep indices in bounds
			if (dropIndex < 0) {
				dropIndex = 0;
				return;
			} else if (dropIndex >= theItems.length) {
				dropIndex = theItems.length - 1;
				return;
			}
			
			var t = listOrder[oldDropIndex];
			listOrder[oldDropIndex] = listOrder[newDropIndex];
			listOrder[newDropIndex] = t;
			
			// swap list items
			var oldDropTarget = theItems.get(oldDropIndex),
				newDropTarget = theItems.get(newDropIndex),
				temp1 = $(oldDropTarget).clone(true);
				temp2 = $(newDropTarget).clone(true);
				
			$(oldDropTarget).replaceWith(temp2)
				.mouseover(li_mouseover)
				.mousedown(dragHandle_mousedown);
			$(newDropTarget).replaceWith(temp1)
				.mouseover(li_mouseover)
				.mousedown(dragHandle_mousedown);
			
			// reset so it is valid on next use
			theItems = $('li.candrag', theList);
			dropTarget = $(theItems.get(newDropIndex));
		}
		
		function y_top(jq) {
			return jq.offset().top;
		}
		
		function y_mid(jq) {
			return (y_top(jq) + y_bot(jq)) / 2
		}
		
		function y_bot(jq) {
			return jq.offset().top + jq.outerHeight();
		}
		
		this.makeDefaultOrder = function() {
			for (var i = 0; i < listOrder.length; i++)
				listOrder[i] = i;
		}
		
		this.restoreOrder = function() { 
			for (var i = 0; i < theItems.length; i++) {
				if (i != listOrder[i]) {
					var k = 0;
					for (; k < listOrder.length; k++)
						if (listOrder[k] == i)
							break;
					swapListItems(i, k);
				}
			}
			theList.trigger('listorderchanged', [theList, listOrder]);
		}
	});
}
})(jQuery);
