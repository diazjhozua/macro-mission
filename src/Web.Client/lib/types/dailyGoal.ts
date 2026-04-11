export interface DailyGoalResult {
  id: string;
  name: string;
  isActive: boolean;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  fiber: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateDailyGoalRequest {
  name: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  fiber: number;
}

// isActive is included on updates so setting a goal active goes through
// the normal PUT rather than a dedicated endpoint.
export interface UpdateDailyGoalRequest extends CreateDailyGoalRequest {
  isActive: boolean;
}
