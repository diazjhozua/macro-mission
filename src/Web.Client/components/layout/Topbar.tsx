"use client";

import { LogOut, User } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useCurrentUser } from "@/lib/hooks/useCurrentUser";
import { useLogout } from "@/lib/hooks/useAuth";

export function Topbar() {
  const { data: currentUser } = useCurrentUser();
  const { mutate: logout, isPending } = useLogout();

  return (
    <header className="h-14 border-b bg-white flex items-center justify-end px-6 shrink-0">
      <DropdownMenu>
        <DropdownMenuTrigger
          render={
            <Button variant="ghost" size="sm" className="gap-2 text-sm">
              <User className="h-4 w-4" />
              {/* Falls back to blank while the query loads — no layout shift. */}
              <span>{currentUser?.nickname ?? ""}</span>
            </Button>
          }
        />

        <DropdownMenuContent align="end" className="w-48">
          {currentUser && (
            <>
              <div className="px-2 py-1.5">
                <p className="text-sm font-medium">{currentUser.firstName} {currentUser.lastName}</p>
                <p className="text-xs text-muted-foreground">@{currentUser.nickname}</p>
              </div>
              <DropdownMenuSeparator />
            </>
          )}

          <DropdownMenuItem
            className="text-destructive focus:text-destructive cursor-pointer"
            disabled={isPending}
            onSelect={() => logout()}
          >
            <LogOut className="h-4 w-4 mr-2" />
            Sign out
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </header>
  );
}
