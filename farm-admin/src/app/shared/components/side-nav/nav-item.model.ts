export interface NavItem {
    displayName: string;
    action: string |undefined;
    route?: string;
    children?: NavItem[];
    url?: string;
  }
