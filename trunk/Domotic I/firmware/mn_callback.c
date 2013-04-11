//-----------------------------------------------------------------------------
// mn_callback.c
//-----------------------------------------------------------------------------
// Copyright 2006 Silicon Laboratories, Inc.
//
// Description:
// 	This file callback functions from the TCP/IP stack.
//  These functions may be edited by the user.
//
//  Callback functions include:
//  1. callback_app_process_packet
//  2. callback_app_server_idle
//  3. callback_app_recv_idle
//  4. callback_app_server_process_packet
//  5. callback_socket_closed (only when TCP is included in the stack)
//  6. callback_socket_empty (only when TCP is included in the stack)
//
// Generated by TCP/IP Configuration Wizard Version 3.23.
//

#include "mn_userconst.h"                      // TCP/IP Library Constants
#include "mn_stackconst.h"                     // TCP/IP Library Constants
#include "mn_errs.h"                           // Library Error Codes
#include "mn_defs.h"                           // Library Type definitions
#include "mn_funcs.h"                          // Library Function Prototypes
#include "VFILE_DIR\index.h"
#include <c8051F340.h>                         // Device-specific SFR Definitions



//------------------------------------------------------------------------------
// Callback Functions
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// callback_app_process_packet
//------------------------------------------------------------------------------
//
// This function is called after any TCP or UDP packet is received.
//
// Parameters:
//    socket_ptr - pointer to the socket that contains the data.
//// Return Values:
//    NEED_IGNORE_PACKET <-94> - The library will not ACK the TCP packet.
//
//    Any other value - The library will ACK the TCP packet.
//
// Note: The return value is ignored if a UDP packet was received.
//
SCHAR callback_app_process_packet(PSOCKET_INFO socket_ptr)
{
   socket_ptr = socket_ptr;   // This statment prevents compiler warning.
   return (1);
}

//------------------------------------------------------------------------------
// callback_app_server_idle
//------------------------------------------------------------------------------
//
// This function is called periodically and when both the receive and transmit
// buffers are empty. This function should only be used for low priority tasks.
// Any high priority tasks should be placed in an interrupt service routine.
//
// Parameters:
//    psocket_ptr - pointer to a socket that can be filled with data.
//
// Return Values:
//    NEED_TO_SEND <-114> - The library will immediately send the data stored in
//    the socket.
//
//    NEED_TO_EXIT <-102> - The mn_server() routine will exit immediately, returning
//    control to the main() routine.
//
//    FALSE <0> - The mn_server() routine will continue to function normally.
//
// Note: The socket pointer may be re-assigned to a different socket.
// (e.g. *psocket_ptr = new_socket_ptr; )
//
/*
SCHAR callback_app_server_idle(PSOCKET_INFO *psocket_ptr)
{

   // Put your code here.

   // If the link goes invalid, exit mn_server().
   if(link_lost)
      return (NEED_TO_EXIT);

   psocket_ptr = psocket_ptr; // This statment prevents compiler warning.
   return (0);
}
*/
//------------------------------------------------------------------------------
// callback_app_recv_idle
//------------------------------------------------------------------------------
//
// This function is called repeatedly while the server is waiting for data.
// This function should only be used for low priority tasks.  Any high priority
// tasks should be placed in an interrupt service routine.
//
// Parameters:
//    none
//
// Return Values:
//    NEED_TO_EXIT <-102> - The server will immedialtely stop waiting for data and
//    will advance to the next state.
//
//    Any other value - The server will continue to wait for data.
//
SCHAR callback_app_recv_idle(void)
{
   // Put your code here.
   return (0);
}

//------------------------------------------------------------------------------
// callback_app_server_process_packet
//------------------------------------------------------------------------------
//
// This function is called after any TCP or UDP packet that is not HTTP or FTP
// received.  HTTP and FTP packets are automatically handled by the server.
//
// Parameters:
//    socket_ptr - pointer to the socket that contains the data.
//
// Return Values:
//    NEED_TO_EXIT <-102> - The mn_server() routine will exit immediately, returning
//    control to the main() routine.
//
//    Any other value - The server will discard the packet.
//
SCHAR callback_app_server_process_packet(PSOCKET_INFO socket_ptr)
{
   socket_ptr = socket_ptr;   // This statment prevents compiler warning.

   return (0);
}


//------------------------------------------------------------------------------
// callback_socket_closed
//------------------------------------------------------------------------------
//
// This function is called after a TCP socket is closed.
//
// Parameters:
//    socket_no - number of the socket that was closed.
//
// Return Values:
//
//    N/A
//
void callback_socket_closed(SCHAR socket_no)
{
	socket_no = socket_no;
}

//------------------------------------------------------------------------------
// callback_socket_empty
//------------------------------------------------------------------------------
//
// This function is called after all data in a TCP socket is sent.
//
// Parameters:
//    socket_ptr - pointer to the socket that is empty.
//
// Return Values:
//
//    N/A
//
void callback_socket_empty(PSOCKET_INFO socket_ptr)
{
	socket_ptr = socket_ptr;
}