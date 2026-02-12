namespace Backend.Dto;

public record StatsResponseDto(List<CourseRoundStatsDto> CourseRounds);

public record CourseRoundStatsDto(
    string Course,
    int Year,
    int Semester,
    int TotalStudents,
    List<StatusCountDto> StatucCounts
);

public record StatusCountDto(string Status, int Count);
