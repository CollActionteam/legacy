export interface IDonation {
  hasIDealPaymentSucceeded: boolean;
  stripePublicKey: string;
}

export interface ISettings {
  googleAnalyticsID: string;
  facebookPixelID: string;
  mailChimpNewsletterListId: string;
  mailChimpUserId: string,
  mailChimpAccount: string,
  mailChimpServer: string,
  stripePublicKey: string;
  externalLoginProviders: string[];
  categories: string[];
  displayPriorities: string[];
  crowdactionStatusses: string[];
}

export interface IDonationEventLog {
  eventData: string;
  id: string;
  user: IUser;
  userId: string;
}

export interface IDonationSubscription {
  canceledAt: string;
  id: string;
  startDate: string;
}

export interface IImageFile {
  date: string;
  description: string;
  filepath: string;
  height: number;
  id: string;
  url: string;
  width: number;
}

export interface ICrowdactionCategory {
  category: string;
}

export interface IMiscellaneous {
  externalLoginProviders: string[];
  festivalCallToActionVisible: boolean;
}

export interface IOrderBy {
  path: string;
  descending: boolean;
}

export enum CrowdactionDisplayPriority {
  TOP = "TOP",
  MEDIUM = "MEDIUM",
  BOTTOM = "BOTTOM",
}

export interface ICrowdaction {
  anonymousUserParticipants: number;
  bannerImage: IImageFile;
  bannerImageFileId: string;
  canSendCrowdactionEmail: boolean;
  categories: ICrowdactionCategory[];
  creatorComments: string;
  description: string;
  descriptionVideoLink: string;
  cardImage: IImageFile;
  cardImageFileId: string;
  descriptiveImage: IImageFile;
  descriptiveImageFileId: string;
  displayPriority: CrowdactionDisplayPriority;
  start: string;
  end: string;
  goal: string;
  id: string;
  isActive: boolean;
  isClosed: boolean;
  isComingSoon: boolean;
  isSuccessfull: boolean;
  isFailed: boolean;
  name: string;
  nameNormalized: string;
  numberCrowdactionEmailsSent: number;
  owner: IUser;
  ownerId: string;
  percentage: number;
  participants: ICrowdactionParticipant[];
  proposal: string;
  remainingTime: number;
  remainingTimeUserFriendly: string;
  status: CrowdactionStatus;
  tags: ICrowdactionTag[];
  target: number;
  totalParticipants: number;
  url: string;
}

export interface ICrowdactionParticipant {
  id: string;
  crowdaction: ICrowdaction;
  subscribedToCrowdactionEmails: boolean;
  unsubscribeToken: string;
  user: IUser;
}

export enum CrowdactionStatus {
  HIDDEN = "HIDDEN",
  RUNNING = "RUNNING",
  DELETED = "DELETED",
}

export enum CrowdactionStatusFilter {
  Open = "OPEN",
  Closed = "CLOSED",
  ComingSoon = "COMING_SOON",
  Active = "ACTIVE"
}

export interface ICrowdactionTag {
  crowdaction: ICrowdaction;
  crowdactionId: string;
  tag: ITag;
  tagId: string;
}

export interface ITag {
  id: string;
  name: string;
  crowdactionTags: ICrowdactionTag[];
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
  participates: ICrowdactionParticipant[];
  crowdactions: ICrowdaction[];
  representsNumberParticipants: number;
  userEvents: IUserEvent[];
  registrationDate: string;
}

export interface IUserEvent {
  eventData: string;
  eventLoggedAt: string;
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

export interface ICrowdactionResult {
  succeeded: boolean;
  crowdaction: ICrowdaction;
  errors: ValidationResult[];
}

export interface ValidationResult {
  errorMessage: string;
  memberNames: string[];
}
