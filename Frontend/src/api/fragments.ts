export const Fragments = {
  crowdactionDetail: `
    id
    name
    description
    url
    categories {
      category
    }
    cardImage {
      id
      description
      url
    }
    bannerImage {
      id
      description
      url
    }
    descriptiveImage {
      id
      description
      url
    }
    owner {
      fullName
      firstName
      lastName
    }
    tags {
      tag {
        name
      }
    }
    goal
    start
    end
    status
    target
    proposal
    creatorComments
    remainingTime
    remainingTimeUserFriendly
    canSendCrowdactionEmail
    displayPriority
    isActive
    isComingSoon
    isClosed
    isSuccessfull
    isFailed
    totalParticipants
    percentage
    numberCrowdactionEmailsSent
  `,
};
