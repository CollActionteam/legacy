export interface IDonation {
  hasIDealPaymentSucceeded: boolean;
  stripePublicKey: string;
}

export interface IDonationEventLog {
  eventData: string;
  id: string;
  user: IUser;
  userId: string;
}

export interface IDonationSubscription {
  canceledAt: Date;
  id: string;
  startDate: Date;
}

export interface IImageFile {
  date: Date;
  description: string;
  filepath: string;
  height: number;
  id: string;
  url: string;
  width: number;
}

export interface IProjectCategory {
  category: string;
}

export interface IMiscellaneous {
  disqusSite: string;
  externalLoginProviders: string[];
  festivalCallToActionVisible: boolean;
}

export interface IOrderBy {
  path: string;
  descending: boolean;
}

export enum ProjectDisplayPriority {
  TOP,
  MEDIUM,
  BOTTOM,
}

export interface IProject {
  anonymousUserParticipants: number;
  bannerImage: IImageFile;
  bannerImageFileId: string;
  canSendProjectEmail: boolean;
  categories: IProjectCategory[];
  creatorComments: string;
  description: string;
  descriptionVideoLink: string;
  descriptiveImage: IImageFile;
  descriptiveImageFileId: string;
  displayPriority: ProjectDisplayPriority;
  start: Date;
  end: Date;
  goal: string;
  id: string;
  isActive: boolean;
  isClosed: boolean;
  isComingSoon: boolean;
  isSuccessfull: boolean;
  isFailed: boolean;
  name: string;
  nameNormalized: string;
  numberProjectEmailSent: number;
  owner: IUser;
  ownerId: string;
  percentage: number;
  participants: IProjectParticipant[];
  proposal: string;
  remainingTime: any;
  status: ProjectStatus;
  tags: IProjectTag[];
  target: number;
  totalParticipants: number;
  url: string;
}

export interface IProjectParticipant {
  project: IProject;
  subscribedToProjectEmails: boolean;
  unsubscribeToken: any;
  user: IUser;
}

export enum ProjectStatus {
  HIDDEN,
  RUNNING,
  SUCCESFULL,
  FAILED,
  DELETED,
}

export enum ProjectStatusFilter {
  Open = "OPEN",
  Closed = "CLOSED",
  ComingSoon = "COMING_SOON",
  Active = "ACTIVE"
}

export interface IProjectTag {
  project: IProject;
  projectId: string;
  tag: ITag;
  tagId: string;
}

export interface ITag {
  id: string;
  name: string;
  projectTags: IProjectTag[];
}

export interface IUser {
  donationEvents: IDonationEventLog[];
  donationSubscriptions: IDonationSubscription[];
  email: string;
  fullName: string | null;
  firstName: string | null;
  lastName: string | null;
  id: string;
  isAdmin: boolean;
  isSubscribedNewsletter: boolean;
  loginProviders: string[];
  participates: IProjectParticipant[];
  projects: IProject[];
  representsNumberParticipants: number;
  userEvents: IUserEvent[];
  registrationDate: Date;
}

export interface IUserEvent {
  eventData: string;
  eventLoggedAt: Date;
  id: string;
  user: IUser;
  userId: string;
}

export interface IWhereExpression {
  path: string;
  comparision: any;
  case: any;
  value: string[];
}

export interface IProjectResult {
  succeeded: boolean;
  project: IProject;
  errors: ValidationResult[];
}

export interface ValidationResult {
  errorMessage: string;
  memberNames: string[];
}
